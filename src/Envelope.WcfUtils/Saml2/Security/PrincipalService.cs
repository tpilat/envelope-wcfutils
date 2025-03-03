﻿using Envelope.Trace;
using Envelope.WcfUtils.Saml2.Config;
using Envelope.WcfUtils.Saml2.Messages;
using Envelope.WcfUtils.Saml2.Serializers;

namespace Envelope.WcfUtils.Saml2.Security;

internal static class PrincipalService
{
	public static async Task<Saml2Principal?> CreatePrincipalAsync(
		AssertionType assertion,
		string assertionRaw,
		AssertionAttributeConfig? attrConfig,
		Func<PrincipalTicketInfo, PrincipalSessionInfo, object?, Task<object?>>? userDataDelegate)
	{
		if (attrConfig == null)
			return null;

		assertion.Attributes.MapAttributeValues(attrConfig);

		var multiValue = assertion.Attributes.GetMultiValue(attrConfig.RolesAttribute);
		object? userData = null;

		var principalTicketInfo = new PrincipalTicketInfo()
		{
			Username = GetUserNameOrNameID(assertion, attrConfig),
			AuthnType = assertion.AuthnContextClassRef,
			SessionIndex = assertion.AuthnStatement.SessionIndex,
			Id = assertion.Subject.NameID.Value,
			IdFormat = assertion.Subject.NameID.Format,
			IdIdp = assertion.Subject.NameID.NameQualifier,
			IdSp = assertion.Subject.NameID.SPNameQualifier,
			ValidTo = assertion.AuthnStatement.SessionNotOnOrAfter
		};

		var principalSessionInfo = new PrincipalSessionInfo()
		{
			Assertion = assertionRaw,
			Attributes = CreateAttributeDictionary(assertion),
			Roles = multiValue
		};

		if (userDataDelegate != null)
			userData = await userDataDelegate.Invoke(principalTicketInfo, principalSessionInfo, null);

		return 
			new Saml2Principal(
				principalTicketInfo,
				principalSessionInfo,
				true,
				null!,
				userData);
	}

	public static Task StorePrincipal(
		Action<DateTime, string, string, Saml2Principal> addCookieToResponse,
		ISerializer serializer,
		Saml2Principal principal,
		ITraceInfo traceInfo,
		Func<string, Saml2Principal, DateTime, Task> sessionStoreDelegate)
	{
		if (addCookieToResponse == null)
			throw new ArgumentNullException(nameof(addCookieToResponse));

		if (serializer == null)
			throw new ArgumentNullException(nameof(serializer));

		if (principal == null)
			throw new ArgumentNullException(nameof(principal));

		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (sessionStoreDelegate == null)
			throw new ArgumentNullException(nameof(sessionStoreDelegate));

		var issueDate = GlobalContext.Instance.Now;
		var userName = principal.Identity.Name!;
		var userData = $"[Saml2Principal]{serializer.Serialize(principal.TicketInfo)}";
		addCookieToResponse(issueDate, userName, userData, principal);
		return sessionStoreDelegate(userData, principal, principal.ValidTo);
	}

	public static async Task<Saml2Principal> LoadPrincipalAsync(
		ISerializer serializer,
		string principalTicketInfoSerialized,
		PrincipalSessionInfo sessionInfo,
		object? userInfo,
		string formsAuthenticationTicketUserData,
		Func<PrincipalTicketInfo, PrincipalSessionInfo, object?, Task<object?>>? userDataDelegate)
	{
		try
		{
			var ticketInfo = serializer.Deserialize<PrincipalTicketInfo>(principalTicketInfoSerialized);

			object? userData = null;
			if (userDataDelegate != null)
				userData = await userDataDelegate.Invoke(ticketInfo!, sessionInfo, userInfo);

			return new Saml2Principal(ticketInfo!, sessionInfo, false, formsAuthenticationTicketUserData, userData);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Unable to restore {nameof(Saml2Principal)}", ex);
		}
	}

	private static Dictionary<string, List<string>> CreateAttributeDictionary(AssertionType assertion)
	{
		var attributeDictionary = new Dictionary<string, List<string>>();

		foreach (var attributeName in assertion.Attributes.GetAttributeNames())
		{
			var multiValue = assertion.Attributes.GetMultiValue(attributeName);
			attributeDictionary.Add(attributeName, multiValue);
		}

		return attributeDictionary;
	}

	private static string? GetUserNameOrNameID(
		AssertionType assertion,
		AssertionAttributeConfig attrConfig)
	{
		var userNameOrNameId = assertion.Attributes.GetValue(attrConfig.UserNameAttribute);

		if (string.IsNullOrEmpty(userNameOrNameId))
			userNameOrNameId = assertion.Subject.NameID?.Value;

		return userNameOrNameId;
	}
}