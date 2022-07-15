using TaleWorlds.Library;

namespace BetterCrafting.ViewModels
{
	public class CraftingMultiplyerViewModel : ViewModel
	{
		private string displayString = "";
		//private int multiplier;

		[DataSourceProperty]
		public string DisplayText
		{
			get
			{
				return displayString;
			}
			set
			{
				this.displayString = value;
				base.OnPropertyChangedWithValue(value, "DisplayText");
			}
		}

		public int multiplier;

		public int GetMultiplier()
		{
			return multiplier;
		}

		public void SetMultiplier(int i)
		{
			multiplier = i;
			string s = "x" + i.ToString();
			this.displayString = s;
			base.OnPropertyChangedWithValue(s, "DisplayText");
		}
	}
}
