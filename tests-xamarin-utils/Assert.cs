using System;

namespace tests
{
	public class Assert
	{
		public Assert()
		{
		}

		public static void AreEqual<T> (T expected, T actual)
		{
			Xunit.Assert.Equal (expected, actual);
		}


		public static void IsTrue (bool condition)
		{
			Xunit.Assert.True (condition);
		}

		public static void IsNotNull (object @object)
		{
			Xunit.Assert.NotNull (@object);
		}
	}
}

