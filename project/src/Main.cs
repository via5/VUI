namespace via5
{
	class VUIMain : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			base.Init();

			VUI.Glue.Set(
				() => manager,
				(s, ps) => string.Format(s, ps),
				(s) => SuperController.LogError(s),
				(s) => SuperController.LogError(s),
				(s) => SuperController.LogError(s),
				(s) => SuperController.LogError(s));

			root_ = new VUI.Root(this);
			root_.ContentPanel.Layout = new VUI.BorderLayout();

			var p = new VUI.Panel(new VUI.HorizontalFlow(
				0, VUI.FlowLayout.AlignCenter | VUI.FlowLayout.AlignVCenter));
			p.Add(new VUI.CheckBox("test"));

			root_.ContentPanel.Add(p, VUI.BorderLayout.Center);
		}

		public void FixedUpdate()
		{
		}

		public void Update()
		{
			root_?.Update();
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
		}

		public static void Main()
		{
		}
	}
}
