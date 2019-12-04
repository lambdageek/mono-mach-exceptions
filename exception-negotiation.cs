using System;
using System.Threading;

public class Test
{
	[System.Runtime.InteropServices.DllImport ("libexception-negotiation-native")]
	private static extern void install_mach_exception_handlers ();

	[System.Runtime.InteropServices.DllImport ("libexception-negotiation-native")]
	private static extern void generate_native_abort ();

	public static int Main (String [] arguments)
	{
		Wrapper wrapper = null;
		install_mach_exception_handlers ();

		use_wrapper (wrapper);

		// same thing but on another thread
		var t0 = new Thread (runner);
		t0.Start ();
		t0.Join ();

		// same thing but while there's a GC happening.
		var t_gc = new Thread (always_collect);
		t_gc.IsBackground = true;
		var t1 = new Thread (runner);
		t_gc.Start();
		t1.Start ();
		t1.Join ();

		// This should generate a crash report.
		try {
			generate_native_abort ();
		} catch (Exception exception) {
			Console.Error.WriteLine (exception);
			return 1;
		}

		return 0;
	}

	[System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
	private static void use_wrapper (Wrapper wrapper)
	{		// This should generate and catch a NullReferenceException.
		try {
			Console.WriteLine (wrapper.value);
		} catch (NullReferenceException exception) {
			Console.Error.WriteLine ("caught an NRE");
			Console.Error.WriteLine (exception);
		}
	}

	private static void always_collect ()
	{
		while (true) {
			GC.Collect (GC.MaxGeneration);
		}
	}

	private static void runner ()
	{
		for (int i = 0; i < 5; ++i) {
			Wrapper wrapper = null;
			use_wrapper (wrapper);
		}
	}

}

class Wrapper
{
	public int value;

	public Wrapper (int value)
	{
		this.value = value;
	}
}
