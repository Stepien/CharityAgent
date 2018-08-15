using System;
using System.IO;
using Xunit;

namespace CharityAgent.Tests
{
	public class ValidateConsoleInputTests
	{
		public ValidateConsoleInputTests()
		{
			StreamWriter standardOut =
			new StreamWriter(Console.OpenStandardOutput())
			{
				AutoFlush = true
			};
			Console.SetOut(standardOut);

		}

		[Fact]
		public void ConverIntFromConsole_GoodInput_ShouldReturnInt5()
		{
			int result;
			using (StringWriter sw = new StringWriter())
			{
				Console.SetOut(sw);

				using (StringReader sr = new StringReader(string.Format("5")))
				{
					System.Console.SetIn(sr);
					result = Program.ConvertIntFromConsole();
				}
			}
			Assert.Equal(5, result);

		}

		[Fact]
		public void ConverIntFromConsole_NullInput_ShouldReturn0()
		{
			int result;
			using (StringWriter sw = new StringWriter())
			{
				Console.SetOut(sw);

				using (StringReader sr = new StringReader(string.Format("")))
				{
					System.Console.SetIn(sr);
					result = Program.ConvertIntFromConsole();
				}
			}

			Assert.Equal(0, result);

		}

		[Fact]
		public void ConverIntFromConsole_NegativeInput_ShouldReturnNegative4()
		{
			int result;
			using (StringWriter sw = new StringWriter())
			{
				Console.SetOut(sw);

				using (StringReader sr = new StringReader(string.Format("-4")))
				{
					System.Console.SetIn(sr);
					result = Program.ConvertIntFromConsole();
				}
			}

			Assert.Equal(-4, result);

		}
	}
}
