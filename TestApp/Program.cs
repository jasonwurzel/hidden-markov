using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = @"X:\Data\!Stuff\hidden-markov\HiddenMarkovTests\ManualTests\Dummy\hmm.txt";
			string serializedHmm = File.ReadAllText(path);
			var hmm = HiddenMarkovModel.CreateFromString(serializedHmm);

			Console.WriteLine("Generating...");
			var random = new Random();
			for (var i = 0; i < 100; i++)
				Console.WriteLine(HiddenMarkovModel.Prototype.Generate('$', (int)Math.Round(Math.Abs(random.NextDouble() * 4 + 4)), 0.09, hmm)); // .slice(0, -1));
		}
	}
}
