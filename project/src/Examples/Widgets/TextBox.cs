namespace via5
{
	class ExampleTextBox : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			var b = new VUI.TextBox("", "placeholder");
			b.Changed += (s) => SuperController.LogError($"changed: '{s}'");
			b.Edited += (s) => SuperController.LogError($"edited: '{s}'");
			b.Submitted += (s) => SuperController.LogError($"submited: '{s}'");

			root_.ContentPanel.Add(b);
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
