namespace via5
{
	class ExampleCheckbox : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			root_.ContentPanel.Add(new VUI.CheckBox(
				"checkbox", (b) => SuperController.LogError($"{b}")));
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
