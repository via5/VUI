using System.Collections.Generic;

namespace via5
{
	class ExampleListView : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.VerticalFlow();

			var list = new VUI.ListView<string>(
				(s) => SuperController.LogError(s));

			for (int i = 0; i < 20; ++i)
				list.AddItem($"{i + 1}");

			root_.ContentPanel.Add(list);
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
