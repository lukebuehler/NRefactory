﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.TypeMembers
{
	[TestFixture]
	public class PropertyDeclarationTests
	{
		[Test]
		public void SimpleGetSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { get {} set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.Getter.IsNull);
			Assert.IsFalse(pd.Setter.IsNull);
		}
		
		[Test]
		public void GetSetPropertyDeclarationWithAccessorModifiers()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { private get {} protected internal set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.Getter.IsNull);
			Assert.IsFalse(pd.Setter.IsNull);
		}
		
		[Test]
		public void SimpleGetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.Getter.IsNull);
			Assert.IsTrue(pd.Setter.IsNull);
		}
		
		[Test]
		public void SimpleSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyProperty { set {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.Getter.IsNull);
			Assert.IsFalse(pd.Setter.IsNull);
		}
		
		[Test]
		public void PropertyRegionTest()
		{
			const string code = "class T {\n\tint Prop {\n\t\tget { return f; }\n\t\tset { f = value; }\n\t}\n}\n";
			int line2Pos = code.IndexOf("\tint Prop");
			int line3Pos = code.IndexOf("\t\tget");
			int line4Pos = code.IndexOf("\t\tset");
			
			CSharpParser parser = new CSharpParser();
			CompilationUnit cu = parser.Parse(new StringReader(code));
			PropertyDeclaration pd = (PropertyDeclaration)cu.Children.Single().GetChildByRole(TypeDeclaration.MemberRole);
			Assert.AreEqual(new AstLocation(2, code.IndexOf("{\n\t\tget") - line2Pos + 1), pd.GetChildByRole(AstNode.Roles.LBrace).StartLocation);
			Assert.AreEqual(new AstLocation(5, 3), pd.EndLocation);
			Assert.AreEqual(new AstLocation(3, code.IndexOf("{ return") - line3Pos + 1), pd.Getter.Body.StartLocation);
			Assert.AreEqual(new AstLocation(3, code.IndexOf("}\n\t\tset") + 1 - line3Pos + 1), pd.Getter.Body.EndLocation);
			Assert.AreEqual(new AstLocation(4, code.IndexOf("{ f =") - line4Pos + 1), pd.Setter.Body.StartLocation);
			Assert.AreEqual(new AstLocation(4, code.IndexOf("}\n\t}") + 1 - line4Pos + 1), pd.Setter.Body.EndLocation);
		}
		
		[Test]
		public void PropertyImplementingInterfaceTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface.MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.Getter.IsNull);
			Assert.IsTrue(pd.Setter.IsNull);
			
			Assert.AreEqual("MyInterface", ((SimpleType)pd.PrivateImplementationType).Identifier);
		}
		
		[Test]
		public void PropertyImplementingGenericInterfaceTest()
		{
			PropertyDeclaration pd = ParseUtilCSharp.ParseTypeMember<PropertyDeclaration>("int MyInterface<string>.MyProperty { get {} } ");
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.Getter.IsNull);
			Assert.IsTrue(pd.Setter.IsNull);
			
			Assert.IsNotNull(new SimpleType { Identifier = "MyInterface", TypeArguments = { new PrimitiveType("string") } }.Match(pd.PrivateImplementationType));
		}
	}
}
