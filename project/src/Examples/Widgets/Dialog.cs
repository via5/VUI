namespace via5
{
	class ExampleDialog : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			root_.ContentPanel.Add(new VUI.Button("test", () =>
			{
				var d = new VUI.MessageDialog(
					root_, VUI.Buttons.OK | VUI.Buttons.Cancel,
					"dialog title", "dialog text");

				d.RunDialog((r) =>
				{
					if (r == VUI.Buttons.OK)
						SuperController.LogError("ok");
					else
						SuperController.LogError("cancel");
				});
			}));
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
