using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TestApp;

namespace HiddenMarkovTests
{
	[TestFixture]
	public class ManualTests
	{
		[Test]
		public void Dummy()
		{
			string path = Path.Combine(TestContext.TestMethodPrivateDir, "hmm.txt");
			string serializedHmm = File.ReadAllText(path);
			var hmm = HiddenMarkovModel.CreateFromString(serializedHmm);

			Console.WriteLine("Generating...");
			var random = new Random();
			for (var i = 0; i < 100; i++) 
				Console.WriteLine(HiddenMarkovModel.Prototype.Generate('$', (int) Math.Round(Math.Abs(random.NextDouble()*4 + 4)), 0.09, hmm)); // .slice(0, -1));
		}
	}
}