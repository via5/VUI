using System.Collections.Generic;

namespace via5
{
	class ExampleComboBox : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			root_.ContentPanel.Add(new VUI.ComboBox<string>(
				new List<string> { "item 1", "item 2", "item 3" },
				(s) => SuperController.LogError(s)));
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
