using ProjectOrganizer.Extensions;
using ProjectOrganizer.Models;
using Xunit;

namespace UnitTestLogic {
	public class ExtensionTests {
		[Theory]
		[InlineData("client")]
		[InlineData("Client")]
		[InlineData("ClIEnt")]
		public void ToEnumTest_Client_Pass(string input) {
			SQLTypeEnum result = input.ToEnum<SQLTypeEnum>();
			Assert.Equal(SQLTypeEnum.Client, result);
		}


		[Theory]
		[InlineData("project")]
		public void ToEnumTest_Project_Pass(string input) {
			SQLTypeEnum result = input.ToEnum<SQLTypeEnum>();
			Assert.Equal(SQLTypeEnum.Project, result);
		}

		[Theory]
		[InlineData("workshift")]
		public void ToEnumTest_Workshift_Pass(string input) {
			SQLTypeEnum result = input.ToEnum<SQLTypeEnum>();
			Assert.Equal(SQLTypeEnum.Workshift, result);
		}

		[Theory]
		[InlineData(" client")]
		public void ToEnumTest_Fail(string input) {
			SQLTypeEnum result = input.ToEnum<SQLTypeEnum>();
			Assert.NotEqual(SQLTypeEnum.Project, result);
		}
	}

}
