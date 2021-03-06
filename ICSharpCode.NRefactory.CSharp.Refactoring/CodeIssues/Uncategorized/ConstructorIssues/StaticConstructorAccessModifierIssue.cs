﻿// 
// StaticConstructorModifierIssue.cs.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Static constructor should be without access modifier",
                       Description = "Static constructor should be without access modifier",
                       Category = IssueCategories.CompilerErrors,
                       Severity = Severity.Error,
                       ResharperDisableKeyword = "StaticConstructorAccessModifier",
                       IssueMarker = IssueMarker.WavedLine)]
	public class StaticConstructorAccessModifierIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			var unit = context.RootNode as SyntaxTree;
			if (unit == null)
				return null;
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<StaticConstructorAccessModifierIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
                : base(ctx)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				if (!constructorDeclaration.HasModifier(Modifiers.Static))
					return;
				if (constructorDeclaration.HasModifier(Modifiers.Public) ||
					constructorDeclaration.HasModifier(Modifiers.Private) ||
					constructorDeclaration.HasModifier(Modifiers.Protected)) {
					AddIssue(constructorDeclaration, ctx.TranslateString("Static constructor cannot take access modifier"));
				}
			}
		}
	}
}
