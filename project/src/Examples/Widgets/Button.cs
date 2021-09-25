namespace via5
{
	class ExampleButton : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			root_.ContentPanel.Add(new VUI.Button(
				"Button", () => SuperController.LogError("button clicked")));

			root_.ContentPanel.Add(new VUI.ToolButton(
				"X", () => SuperController.LogError("tool button clicked")));
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
