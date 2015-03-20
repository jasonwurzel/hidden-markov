using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HiddenMarkovTests
{
	public class MyStackFrame
	{
		/// <summary>
		///     Liefere alle Vorfahren-Stackframes als Linq-fähiges Enumerable
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<StackFrame> Ancestors()
		{
			StackFrame callStack = null;
			var i = 1;

			do
			{
				callStack = new StackFrame(i++, true);
				if (callStack != null && callStack.GetMethod() != null)
					yield return callStack;
			} while (callStack != null && callStack.GetMethod() != null);
		}

		/// <summary>
		///     Finde ersten StackFrame, dessen Klasse gegebenes Attribute hat
		/// </summary>
		public static StackFrame FindTypeAttribute(Type attributeType)
		{
			return Ancestors().FirstOrDefault(
				s => s.GetMethod().ReflectedType.IsDefined(attributeType, false));
		}

		/// <summary>
		///     Finde ersten StackFrame, dessen Methode gegebenes Attribute hat
		/// </summary>
		public static StackFrame FindMethodAttribute(Type attributeType)
		{
			return Ancestors().FirstOrDefault(
				s => s.GetMethod().IsDefined(attributeType, false));
		}
	}
}