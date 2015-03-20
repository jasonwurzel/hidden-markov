using System;
using System.IO;
using NUnit.Framework;

namespace HiddenMarkovTests
{
	public static class TestContext
	{
		/// <summary>
		///     Name der Fixture
		/// </summary>
		public static string TestFixtureName
		{
			get
			{
				return MyStackFrame.FindTypeAttribute(typeof (TestFixtureAttribute))
					.GetMethod().ReflectedType.Name;
			}
		}

		/// <summary>
		///     FullName der Fixture
		/// </summary>
		public static string TestFixtureFullName
		{
			get
			{
				return MyStackFrame.FindTypeAttribute(typeof (TestFixtureAttribute))
					.GetMethod().ReflectedType.FullName;
			}
		}

		/// <summary>
		///     Name der Testfunktion
		/// </summary>
		public static string TestMethodName
		{
			get
			{
				var frame = MyStackFrame.FindMethodAttribute(typeof (TestAttribute));
				if (frame != null)
					return frame.GetMethod().Name;
				frame = MyStackFrame.FindMethodAttribute(typeof (TestCaseAttribute));
				if (frame != null)
					return frame.GetMethod().Name;
				try
				{
					return NUnit.Framework.TestContext.CurrentContext.Test.Name;
				}
				catch (NullReferenceException)
				{
					// Der Aufruf aus dem Fixture-Setup heraus muss möglich sein
					return null;
				}
			}
		}

		public static string TestFixturePrivateDir
		{
			get
			{
				// Die Klasse mit dem Attribute [TestFixture]
				var stack = MyStackFrame.FindTypeAttribute(typeof (TestFixtureAttribute));
				var className = stack.GetMethod().ReflectedType.Name;
				return Path.Combine(Path.GetDirectoryName(stack.GetFileName()), className);
			}
		}

		public static string TestMethodPrivateDir
		{
			get { return TestFixturePrivateDir.AddPath(Path.GetFileNameWithoutExtension(TestMethodName)); }
		}
	}
}