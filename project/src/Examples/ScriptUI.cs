namespace via5
{
	class ExampleScriptUI : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			root_.ContentPanel.Add(new VUI.Button(
				"test", () => SuperController.LogError("clicked")));
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
