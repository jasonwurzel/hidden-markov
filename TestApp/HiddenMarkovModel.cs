using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestApp
{
	public class HiddenMarkovModel
	{
		/// <summary>
		/// </summary>
		/// <param name="alphabet">Whats this? TODO: find eloquent name</param>
		/// <param name="numberOfNodes"></param>
		public HiddenMarkovModel(string alphabet, int numberOfNodes)
		{
			Nodes = new List<Node>();
			Init = new List<double>();
			Alphabet = alphabet;

			for (var i = 0; i < numberOfNodes; i++)
			{
				var obj = new Node
						{
							Next = new List<double>(),
							Prob = new List<double>()
						};

				// Is this correct? (=> int/float?)
				for (var j = 0; j < numberOfNodes; j++) obj.Next.Add(1.0/numberOfNodes);
				for (var j = 0; j < alphabet.Length; j++) obj.Prob.Add(1.0/alphabet.Length);
				Nodes.Add(obj);
				Init.Add(1.0/numberOfNodes);
			}

			// random init values
			for (var k = 0; k < 3*numberOfNodes; k++)
			{
				var random = new Random();
				var i = random.Next()*numberOfNodes;
				var j = random.Next()*numberOfNodes;
				if (i == j) continue;
				if (Init[i] + Init[j] > 0.9) continue;
				var obj = Init[i]*random.NextDouble();
				Init[i] -= obj;
				Init[j] += obj;
			}
		}

		public List<Node> Nodes { get; set; }
		public List<double> Init { get; set; }
		public string Alphabet { get; set; }

		public static HiddenMarkovModel CreateFromString(string jsonData)
		{
			var hmm = jsonData.DeserializeFromJsonString<HiddenMarkovModel>();
			// TODO: why this step???
			var hmmReturn = new HiddenMarkovModel(hmm.Alphabet, hmm.Nodes.Count);
			hmmReturn.Nodes = hmm.Nodes;
			hmmReturn.Init = hmm.Init;
			hmmReturn.Alphabet = hmm.Alphabet;

			return hmmReturn;
		}

		public void TrainWithString(string trainingString, double rate)
		{
			if (rate == 0) rate = 0.1;
			var alpha = new List<List<double>>();
			var beta = new List<List<double>>();
			var gamma = new List<List<double>>();
			var kappa = new List<List<List<double>>>();
			var input = new List<int>();
			int i, j, k, l;
			double sum;
			var alphabet = Alphabet;
			var nodes = Nodes;
			var init = Init;

			// E1
			for (i = 0; i < trainingString.Length; i++)
			{
				alpha[i] = new List<double>();
				beta[i] = new List<double>();
				gamma[i] = new List<double>();
				if (i < trainingString.Length - 1) kappa[i] = new List<List<double>>();
				input.Add(alphabet.IndexOf(trainingString[i]));
				if (input[i] == -1) throw new InvalidOperationException("Invalid character: " + trainingString[i]);

				for (j = 0; j < nodes.Count; j++)
				{
					if (i == 0)
					{
						alpha[0][j] = init[j]*nodes[j].Prob[input[0]];
					}
					else
					{
						sum = 0;
						for (k = 0; k < nodes.Count; k++) sum += alpha[i - 1][k]*nodes[k].Next[j];
						alpha[i][j] = sum*nodes[j].Prob[input[i]];
					}
				}
				for (i = trainingString.Length; i-- > 0;)
				{
					for (j = 0; j < nodes.Count; j++)
					{
						if (i == trainingString.Length - 1)
						{
							beta[i][j] = 1;
						}
						else
						{
							beta[i][j] = 0;
							for (k = 0; k < nodes.Count; k++)
								beta[i][j] += nodes[j].Next[k]*nodes[k].Prob[input[i + 1]]*beta[i + 1][k];
						}
					}
				}

				// E2
				for (i = 0; i < trainingString.Length; i++)
				{
					sum = 0;
					for (k = 0; k < nodes.Count; k++) sum += alpha[i][k]*beta[i][k];
					for (j = 0; j < nodes.Count; j++)
					{
						gamma[i][j] = alpha[i][j]*beta[i][j]/sum;
					}
					if (i == trainingString.Length - 1) break;
					sum = 0;
					for (j = 0; j < nodes.Count; j++)
						for (k = 0; k < nodes.Count; k++)
						{
							sum += alpha[i][j]*nodes[j].Next[k]*nodes[k].Prob[input[i + 1]]*beta[i + 1][k];
						}
					for (j = 0; j < nodes.Count; j++)
						for (kappa[i][j] = new List<double>(),k = 0; k < nodes.Count; k++)
						{
							kappa[i][j][k] = alpha[i][j]*nodes[j].Next[k]*nodes[k].Prob[input[i + 1]]*beta[i + 1][k]/sum;
						}
				}

				// M
				var a = new List<List<double>>();
				var b = new List<List<double>>();
				var p = new List<double>();
				double del = 0;
				for (i = 0; i < nodes.Count; i++)
				{
					a[i] = new List<double>();
					b[i] = new List<double>();
					for (k = 0, sum = 0; k < trainingString.Length - 1; k++) sum += gamma[k][i];
					for (j = 0; j < nodes.Count; j++)
					{
						for (k = 0, a[i][j] = 0; k < trainingString.Length - 1; k++) a[i][j] += kappa[k][i][j];
						a[i][j] /= sum;

						del = a[i][j] - nodes[i].Next[j];
						nodes[i].Next[j] += del*rate;
					}
					sum += gamma[trainingString.Length - 1][i];
					for (j = 0; j < alphabet.Length; j++)
					{
						for (k = 0, b[i][j] = 0; k < trainingString.Length; k++) if (input[k] == j) b[i][j] += gamma[k][i];
						b[i][j] /= sum;
						del = b[i][j] - nodes[i].Prob[j];
						nodes[i].Prob[j] += del*rate;
					}
					p[i] = gamma[0][i];

					del = p[i] - init[i];
					init[i] += del*rate;
				}
			}
		}

		// Train these words.
		public void TrainWords(List<string> words, double amount)
		{
			foreach (var word in words)
			{
				Console.WriteLine("Training: " + word);
				TrainWithString(word, amount);
			}
		}

		/// <summary>
		///     Gibt eine Zeichenfolge zurück, die das aktuelle Objekt darstellt.
		/// </summary>
		/// <returns>
		///     Eine Zeichenfolge, die das aktuelle Objekt darstellt.
		/// </returns>
		public override string ToString() { return this.SerializeToJsonString(); }

		public class Prototype
		{
			private readonly HiddenMarkovModel _hmm;
			public Prototype(HiddenMarkovModel hmm) { _hmm = hmm; }

			/// <summary>
			///     Generate string from HMM which ends with stop, and has min. length len. q determines minimum quality.
			/// </summary>
			/// <returns></returns>
			public string Generate(char stop, int length, double quality)
			{
				Func<List<double>, int> choose = s1 =>
												{
													var x = new Random().NextDouble();
													var i = 0;
													for (; i < s1.Count && x > 0; i++)
														x -= s1[i];
													return --i;
												};

				var resultingString = "";
				var c = '';
				Node node;

				var pos = choose(_hmm.Init);
				do
				{
					resultingString = "";
					do
					{
						node = _hmm.Nodes[pos];
						c = _hmm.Alphabet[choose(node.Prob)];
						if (resultingString.Length < length && c == stop)
						{
							Debugger.Break();
							// WTF???
							//c = stop + 'x';
						}
						else
						{
							resultingString += c;
							pos = choose(node.Next);
						}
					} while (c != stop);
				} while (quality > 0 && Math.Pow(evaluate(resultingString), 1/resultingString.Length) < quality);
				return resultingString;
			}

			private double evaluate(string resultingString)
			{
				var alpha = new List<List<double>>();

				int i = 0, j, k;
				double sum = 0;
				int input;


				for (i = 0; i < resultingString.Length; i++)
				{
					alpha[i] = new List<double>();
					input = _hmm.Alphabet.IndexOf(resultingString[i]);
					if (input == -1) throw new InvalidOperationException("Invalid character: " + resultingString[i]);
					for (j = 0; j < _hmm.Nodes.Count; j++)
					{
						if (i == 0)
						{
							alpha[0][j] = _hmm.Init[j]*_hmm.Nodes[j].Prob[input];
						}
						else
						{
							for (k = 0, sum = 0; k < _hmm.Nodes.Count; k++) sum += alpha[i - 1][k]*_hmm.Nodes[k].Next[j];
							alpha[i][j] = sum*_hmm.Nodes[j].Prob[input];
						}
					}
				}
				for (sum = i = 0; i < _hmm.Nodes.Count; i++) sum += alpha[resultingString.Length - 1][i];
				return sum;
			}
		}
	}

	public class Node
	{
		public List<double> Next { get; set; }
		public List<double> Prob { get; set; }
	}
}