namespace via5
{
	class ExampleColorPicker : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			root_.ContentPanel.Add(new VUI.ColorPicker(
				"pick a color", (c) => SuperController.LogError($"{c}")));
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
