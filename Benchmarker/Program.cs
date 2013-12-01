using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;
using System.Diagnostics;

namespace Benchmarker
{
    class Program
    {
		static void ShowProcessBits ()
		{
			Console.WriteLine ("Benckmark running in 64bit mode: {0}", Environment.Is64BitProcess);
		}

		//TODO: DebuggableAttribute information is not available under Mono, so this code is x-plat moot 
		static void AssemblyDebugInfo ()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies ();
			// The assembly is in a variable named assembly.
			foreach(var assembly in assemblies) 
			{
				var attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), true);

				// If the array is null, or has length of zero, then it is not debuggable.
				if (!(attributes == null || attributes.Length == 0))
				{
					// It is debuggable, figure out the level.
					DebuggableAttribute debug = (DebuggableAttribute) attributes[0];
					Console.WriteLine (assembly.FullName);
					Console.WriteLine (debug.DebuggingFlags);
					Console.WriteLine (debug.IsJITOptimizerDisabled);
					Console.WriteLine (debug.IsJITTrackingEnabled);
					Console.WriteLine ("~~~~");
				}
				object[] attribs = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);
				// If the 'DebuggableAttribute' is not found then it is definitely an OPTIMIZED build
				if (attribs.Length > 0)
				{
					// Just because the 'DebuggableAttribute' is found doesn't necessarily mean
					// it's a DEBUG build; we have to check the JIT Optimization flag
					// i.e. it could have the "generate PDB" checked but have JIT Optimization enabled
					DebuggableAttribute debuggableAttribute = attribs[0] as DebuggableAttribute;
					if (debuggableAttribute != null)
					{
						var HasDebuggableAttribute = true;
						var IsJITOptimized = !debuggableAttribute.IsJITOptimizerDisabled;
						var BuildType = debuggableAttribute.IsJITOptimizerDisabled ? "Debug" : "Release";

						// check for Debug Output "full" or "pdb-only"
						var DebugOutput = (debuggableAttribute.DebuggingFlags & 
							DebuggableAttribute.DebuggingModes.Default) != 
						  DebuggableAttribute.DebuggingModes.None 
						  ? "Full" : "pdb-only";
					}
				}
				else
				{
					var IsJITOptimized = true;
					var BuildType = "Release";
				}

			}
		}

		static void RunBenchMarkFiles (string version, int repeat = 5)
		{
			// Up the thread priority so nothing gets in the way of the benchmarking.
			System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			// Sunspider.
			var files = Directory.GetFiles (@"../../Files/" + version, "*.js");
			var results = new Series[files.Length];
			Console.WriteLine ("{0} Benchmark", version);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			for (int j = 0; j < repeat; j++) {
				Console.Write ("Pass #{0} :", j + 1);
				for (int i = 0; i < files.Length; i++) {
					// Get the script path.
					var path = files [i];
					// Indicate we are running the test.
					//					Console.Write (".");
					Console.WriteLine ("Running {0}... ", Path.GetFileNameWithoutExtension (path));
					// Initialize the script engine.
					var engine = new ScriptEngine ();
					engine.EnableDebugging = false;
					// If running Octane, include the base objects
					var includes = Directory.GetFiles (@"../../Files/" + version, "*.js");
					foreach (var include in includes)
					{
						if (include.Contains("box2d.js") || include.Contains("base.js"))
						{
							engine.ExecuteFile(include);
						}
					}
					var timer = new System.Diagnostics.Stopwatch ();
					// Load the javascript source into a string (so no I/O during benchmarking).
					var script = File.ReadAllText (path);
					try {
						// Start timing.
						timer.Start();
						// Execute the script.
						engine.Execute (new StringScriptSource (script, path));
						// Stop timing.
						double elapsed = timer.Elapsed.TotalMilliseconds;
						// Add the result to the prior result (but throw away the first result).
						if (results [i] == null)
							results [i] = new Series ();
						else
							results [i].AddSample (elapsed);
					} catch (Exception error) {
						// Show benchmarks that throw exceptions
						Console.WriteLine ("{0} threw exception", path);
						Console.WriteLine ("--- {0}", error.Message);
						if (results [i] == null)
							results [i] = new Series ();
						else
							results [i].AddSample (9999999);
					} finally {
						timer = null;
						engine = null;
						script = null;
						GC.Collect ();
						GC.WaitForPendingFinalizers ();
						GC.Collect ();
					}
				}
				Console.WriteLine ();
			}
			System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;

			// Print the results.
			Console.WriteLine ("{0} : Summary", version);
			Console.WriteLine ();
			double total = 0;
			for (int i = 0; i < files.Length; i++) {
				total += results [i].Mean;
				Console.WriteLine ("{0} - {1:n1}ms \u00B1 {2:n1}ms", Path.GetFileNameWithoutExtension (files [i]), results [i].Mean, results [i].StandardDeviation);
			}
			Console.WriteLine ("Total elapsed time: {0:n1}ms", total);
			Console.WriteLine ();
		}

		static void RunOctaneBenchMarkFiles (string version)
		{
			// Up the thread priority so nothing gets in the way of the benchmarking.
			System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			//var files = Directory.GetFiles (@"../../Files/" + version, "*.js");
			Console.WriteLine ("{0} Benchmark", version);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Console.WriteLine ("Running Octane... ");
			// Initialize the script engine.
			var engine = new ScriptEngine ();
			engine.EnableDebugging = false;
			var includes = Directory.GetFiles (@"../../Files/" + version, "*.js");
			foreach (var include in includes)
			{
				//if (include.Contains("box2d.js") || include.Contains("base.js"))
				if (!include.Contains("run.js"))
				{
					Console.WriteLine("~~~ Loading {0} ~~~", include);
					engine.ExecuteFile(include);
				}
			}
			// Load the javascript source into a string (so no I/O during benchmarking).
			try {
				engine.SetGlobalFunction("WriteLine", new Action<string>((outString) => Console.WriteLine(outString)));
				//						engine.Execute (new StringScriptSource (script, path));
				engine.Execute(@"
					var success = true;

					function print(result) {
					  WriteLine(result);
					}

					function PrintResult(name, result) {
					  print(name + ': ' + result);
					}

					function PrintError(name, error) {
					  PrintResult(name, error);
					  success = false;
					}

					function PrintScore(score) {
					  if (success) {
					    print('----');
					    print('Score (version ' + BenchmarkSuite.version + '): ' + score);
					    print('');
					  }
					}

					BenchmarkSuite.RunSuites({ NotifyResult: PrintResult,
					                           NotifyError: PrintError,
					                           NotifyScore: PrintScore });
				");
			} catch (Exception error) {
				// Show benchmark that throws engine exception
				Console.WriteLine ("~~~ {0}", error.Message);
			} finally {
				engine = null;
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				GC.Collect ();
			}
			System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
		}

        private class Series
        {
            private List<double> samples = new List<double>();

            public void AddSample(double value)
            {
                this.samples.Add(value);
            }

            public double Mean
            {
                get
                {
                    double total = 0;
                    foreach (var sample in this.samples)
                        total += sample;
                    return total / this.samples.Count;
                }
            }

            public double StandardDeviation
            {
                get
                {
                    double mean = this.Mean;
                    double result = 0;
                    foreach (var sample in this.samples)
                        result += (sample - mean) * (sample - mean);
                    result /= this.samples.Count - 1;
                    result = Math.Sqrt(result);
                    return result;
                }
            }
        }

        static void Main(string[] args)
        {
			var loops = 2; // One warmup, plus at least one more
			if (args.Length > 0) {
				loops = int.Parse (args [0]);
			} {
			} 
			ShowProcessBits();
			//RunBenchMarkFiles ( "v8", loops );
			RunBenchMarkFiles ( "sunspider-1.0.2", loops );
			//RunBenchMarkFiles ( "sunspider-0.9.1", loops );
			//RunOctaneBenchMarkFiles ("octane");
        }
    }
}
