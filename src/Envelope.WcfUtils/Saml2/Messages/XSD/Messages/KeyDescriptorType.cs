﻿// Decompiled with JetBrains decompiler
// Type: Envelope.WcfUtils.Saml2.Messages.KeyDescriptorType
// Assembly: RBTeam.Saml2Lib, Version=4.7.2.1, Culture=neutral, PublicKeyToken=443400c129882e90
// MVID: 625824D1-DBAD-407F-86EE-CFDC4EF71CC8
// Assembly location: C:\Code\Disig\Saml2Module\bin\RBTeam.Saml2Lib.dll
// XML documentation location: C:\Code\Disig\Saml2Module\bin\RBTeam.Saml2Lib.xml

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Envelope.WcfUtils.Saml2.Messages
{
  /// <remarks />
  [GeneratedCode("xsd", "4.0.30319.1")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
  [System.Xml.Serialization.XmlRoot("KeyDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
  [Serializable]
  public class KeyDescriptorType
  {
    private KeyInfoType keyInfoField;
    private EncryptionMethodType[] encryptionMethodField;
    private KeyTypes useField;
    private bool useFieldSpecified;

    /// <remarks />
    [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public KeyInfoType KeyInfo
    {
      get => this.keyInfoField;
      set => this.keyInfoField = value;
    }

    /// <remarks />
    [XmlElement("EncryptionMethod")]
    public EncryptionMethodType[] EncryptionMethod
    {
      get => this.encryptionMethodField;
      set => this.encryptionMethodField = value;
    }

    /// <remarks />
    [XmlAttribute]
    public KeyTypes use
    {
      get => this.useField;
      set => this.useField = value;
    }

    /// <remarks />
    [XmlIgnore]
    public bool useSpecified
    {
      get => this.useFieldSpecified;
      set => this.useFieldSpecified = value;
    }
  }
}
