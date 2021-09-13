﻿using UnityEngine.UI;
using UnityEngine;

namespace VUI
{
	interface IRootSupport
	{
		bool Init();
		void Destroy();

		Rectangle Bounds { get; }
		Transform RootParent { get; }

		Point ToLocal(Vector2 v);
	}


	abstract class BasicRootSupport : IRootSupport
	{
		private Canvas canvas_ = null;
		private Rectangle bounds_ = Rectangle.Zero;
		private float topOffset_ = 0;

		public virtual Rectangle Bounds
		{
			get { return bounds_; }
		}

		public abstract Transform RootParent { get; }

		public abstract void Destroy();
		public abstract bool Init();

		public virtual Point ToLocal(Vector2 v)
		{
			if (canvas_ == null)
			{
				canvas_ = GetCanvas();
				if (canvas_ == null)
					return Root.NoMousePos;
			}

			Vector2 pp;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				canvas_.transform as RectTransform, v,
				canvas_.worldCamera, out pp);

			pp.x = bounds_.Left + bounds_.Width / 2 + pp.x;
			pp.y = bounds_.Top + (bounds_.Height - pp.y + topOffset_);

			return new Point(pp.x, pp.y);
		}

		protected void SetBounds(Rectangle r, float topOffset)
		{
			bounds_ = r;
			topOffset_ = topOffset;
		}

		protected abstract Canvas GetCanvas();
	}


	class ScriptUIRootSupport : BasicRootSupport
	{
		private MVRScript s_ = null;
		private MVRScriptUI sui_ = null;

		public ScriptUIRootSupport(MVRScript s)
		{
			s_ = s;
		}

		public ScriptUIRootSupport(MVRScriptUI sui)
		{
			sui_ = sui;
		}

		public override Transform RootParent
		{
			get { return sui_?.fullWidthUIContent; }
		}

		public override bool Init()
		{
			if (sui_ == null)
			{
				if (s_.UITransform == null)
				{
					Glue.LogVerbose("scriptui support: not ready, no UITransform");
					return false;
				}

				sui_ = s_.UITransform.GetComponentInChildren<MVRScriptUI>();
				if (sui_ == null)
				{
					Glue.LogVerbose("scriptui support: not ready, no scriptui");
					return false;
				}
			}

			var scrollView = sui_.GetComponentInChildren<ScrollRect>();
			var scrollViewRT = scrollView.GetComponent<RectTransform>();

			if (scrollViewRT.rect.width <= 0 || scrollViewRT.rect.height <= 0)
			{
				Glue.LogVerbose(
					$"scriptui support: not ready, scroll view size is " +
					$"{scrollViewRT.rect}");

				return false;
			}

			Glue.LogVerbose("scriptui support: ready, initing");

			var bounds = Rectangle.FromPoints(
					1, 1,
					scrollViewRT.rect.width - 3,
					scrollViewRT.rect.height - 3);

			var topOffset = scrollViewRT.offsetMin.y - scrollViewRT.offsetMax.y;

			SetBounds(bounds, topOffset);
			Style.SetupRoot(sui_);

			return true;
		}

		public override void Destroy()
		{
			if (sui_ != null)
				Style.RevertRoot(sui_);
		}

		protected override Canvas GetCanvas()
		{
			return sui_?.GetComponentInChildren<Image>()?.canvas;
		}
	}


	// vr, moves with the head
	//
	class VRTopHudRootSupport : BasicRootSupport
	{
		private Vector3 offset_;
		private Vector2 pos_, size_;
		private GameObject fullscreenPanel_ = null;
		private GameObject hudPanel_ = null;
		private Canvas canvas_ = null;

		public VRTopHudRootSupport(Vector3 offset, Vector2 pos, Vector2 size)
		{
			offset_ = offset;
			pos_ = pos;
			size_ = size;
		}

		public override Transform RootParent
		{
			get { return hudPanel_.transform; }
		}

		public override bool Init()
		{
			CreateFullscreenPanel(Camera.main.transform);
			CreateHudPanel();

			var rt = RootParent.GetComponent<RectTransform>();

			var topOffset = rt.offsetMin.y - rt.offsetMax.y;
			var bounds = Rectangle.FromPoints(
				0, 0, rt.rect.width, rt.rect.height);

			SetBounds(bounds, topOffset);

			return true;
		}

		public override void Destroy()
		{
			if (canvas_ != null)
				SuperController.singleton.RemoveCanvas(canvas_);

			Object.Destroy(fullscreenPanel_);
			fullscreenPanel_ = null;
		}

		private void CreateFullscreenPanel(Transform parent)
		{
			fullscreenPanel_ = new GameObject();
			fullscreenPanel_.transform.SetParent(parent, false);

			canvas_ = fullscreenPanel_.AddComponent<Canvas>();
			var cr = fullscreenPanel_.AddComponent<CanvasRenderer>();
			var cs = fullscreenPanel_.AddComponent<CanvasScaler>();
			var rt = fullscreenPanel_.AddComponent<RectTransform>();
			if (rt == null)
				rt = fullscreenPanel_.GetComponent<RectTransform>();

			canvas_.renderMode = RenderMode.WorldSpace;
			canvas_.worldCamera = Camera.main;
			fullscreenPanel_.transform.position = parent.position + offset_;


			var bg = fullscreenPanel_.AddComponent<Image>();
			bg.color = new Color(0, 0, 0, 0);
			bg.raycastTarget = false;

			var rc = fullscreenPanel_.AddComponent<GraphicRaycaster>();
			rc.ignoreReversedGraphics = false;

			rt.offsetMin = pos_;
			rt.offsetMax = size_;
			rt.anchoredPosition = new Vector2(0, 0);
			rt.anchorMin = new Vector2(0, 1);
			rt.anchorMax = new Vector2(0, 1);
			rt.pivot = new Vector2(0.5f, 0);
			rt.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);

			SuperController.singleton.AddCanvas(canvas_);
		}

		private void CreateHudPanel()
		{
			hudPanel_ = new GameObject();
			hudPanel_.transform.SetParent(fullscreenPanel_.transform, false);

			var bg = hudPanel_.AddComponent<Image>();
			var rt = hudPanel_.AddComponent<RectTransform>();
			if (rt == null)
				rt = hudPanel_.GetComponent<RectTransform>();

			bg.color = new Color(0, 0, 0, 0.8f);
			bg.raycastTarget = true;

			rt.offsetMin = new Vector2(10, 10);
			rt.offsetMax = new Vector2(0, 0);
			rt.anchoredPosition = new Vector2(0, 0);
			rt.anchorMin = new Vector2(0, 0);
			rt.anchorMax = new Vector2(1, 1);
			rt.pivot = new Vector2(0.5f, 0);
		}

		protected override Canvas GetCanvas()
		{
			return canvas_;
		}
	}


	class FaceCamera : MonoBehaviour
	{
		public void LateUpdate()
		{
			var c = Camera.main.transform;
			transform.LookAt(c.position);
		}
	}


	// vr, attaches to a hand
	//
	class VRHandRootSupport : BasicRootSupport
	{
		public const int LeftHand = 0;
		public const int RightHand = 1;

		private Vector3 offset_;
		private Vector2 pos_, size_;
		private GameObject fullscreenPanel_ = null;
		private GameObject hudPanel_ = null;
		private Canvas canvas_ = null;
		private int hand_ = LeftHand;

		public VRHandRootSupport(int hand, Vector3 offset, Vector2 pos, Vector2 size)
		{
			hand_ = hand;
			offset_ = offset;
			pos_ = pos;
			size_ = size;
		}

		public override Transform RootParent
		{
			get { return hudPanel_.transform; }
		}

		public Transform HandTransform
		{
			get
			{
				return (hand_ == LeftHand) ?
					SuperController.singleton.leftHand :
					SuperController.singleton.rightHand;
			}
		}

		public override bool Init()
		{
			CreateFullscreenPanel(HandTransform);
			CreateHudPanel();
			return true;
		}

		public override void Destroy()
		{
			if (canvas_ != null)
				SuperController.singleton.RemoveCanvas(canvas_);

			Object.Destroy(fullscreenPanel_);
			fullscreenPanel_ = null;
		}

		public void Attach(int hand)
		{
			hand_ = hand;
			Attach(HandTransform);
		}

		private void Attach(Transform parent)
		{
			fullscreenPanel_.transform.SetParent(parent, false);
			fullscreenPanel_.transform.position = parent.position + offset_;
		}

		private void CreateFullscreenPanel(Transform parent)
		{
			fullscreenPanel_ = new GameObject();
			fullscreenPanel_.transform.SetParent(parent, false);

			canvas_ = fullscreenPanel_.AddComponent<Canvas>();
			var cr = fullscreenPanel_.AddComponent<CanvasRenderer>();
			var cs = fullscreenPanel_.AddComponent<CanvasScaler>();
			var rt = fullscreenPanel_.AddComponent<RectTransform>();
			if (rt == null)
				rt = fullscreenPanel_.GetComponent<RectTransform>();

			canvas_.renderMode = RenderMode.WorldSpace;
			canvas_.worldCamera = Camera.main;
			fullscreenPanel_.transform.position = parent.position + offset_;


			var bg = fullscreenPanel_.AddComponent<Image>();
			bg.color = new Color(0, 0, 0, 0);
			bg.raycastTarget = false;

			var rc = fullscreenPanel_.AddComponent<GraphicRaycaster>();
			var fc = fullscreenPanel_.AddComponent<FaceCamera>();

			float w = size_.x;
			float h = size_.y;
			float yoffset = 0;
			float s = 0.1f;

			rt.anchorMin = new Vector2(0.5f, 1);
			rt.anchorMax = new Vector2(0.5f, 1);
			rt.offsetMin = new Vector2(-w / 2, -(h + yoffset));
			rt.offsetMax = new Vector2(w / 2, -yoffset);
			rt.anchoredPosition3D = new Vector3(0, 0, 0);

			//rt.anchoredPosition = new Vector2(0.5f, 0.5f);
			rt.localPosition = new Vector3(0, 0.08f, -0.05f);
			rt.localScale = new Vector3(-s / w, s / w, s / w);

			SuperController.singleton.AddCanvas(canvas_);
		}

		private void CreateHudPanel()
		{
			hudPanel_ = new GameObject();
			hudPanel_.transform.SetParent(fullscreenPanel_.transform, false);

			var bg = hudPanel_.AddComponent<Image>();
			var rt = hudPanel_.AddComponent<RectTransform>();
			if (rt == null)
				rt = hudPanel_.GetComponent<RectTransform>();

			bg.color = new Color(0, 0, 0, 0.8f);
			bg.raycastTarget = true;

			rt.offsetMin = new Vector2(10, 10);
			rt.offsetMax = new Vector2(0, 0);
			rt.anchoredPosition = new Vector2(0, 0);
			rt.anchorMin = new Vector2(0, 0);
			rt.anchorMax = new Vector2(1, 1);
			rt.pivot = new Vector2(0.5f, 0);
		}

		protected override Canvas GetCanvas()
		{
			return canvas_;
		}
	}


	// desktop
	//
	class OverlayRootSupport : BasicRootSupport
	{
		private float topOffset_;
		private float width_, height_;

		private GameObject panel_ = null;
		private GameObject ui_ = null;
		private Canvas canvas_ = null;

		public OverlayRootSupport(float topOffset, float width, float height)
		{
			topOffset_ = topOffset;
			width_ = width;
			height_ = height;
		}

		public override Transform RootParent
		{
			get { return ui_.transform; }
		}

		public override bool Init()
		{
			panel_ = new GameObject("OverlayRootSupport");

			canvas_ = panel_.AddComponent<Canvas>();
			panel_.AddComponent<CanvasRenderer>();
			panel_.AddComponent<CanvasScaler>();

			canvas_.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas_.gameObject.AddComponent<GraphicRaycaster>();
			canvas_.scaleFactor = 0.5f;
			canvas_.pixelPerfect = true;

			ui_ = new GameObject("OverlayRootSupportUI");
			ui_.transform.SetParent(panel_.transform, false);
			var rt = ui_.AddComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 1);
			rt.anchorMax = new Vector2(0.5f, 1);
			rt.offsetMin = new Vector2(-width_ / 2, -(height_ + topOffset_));
			rt.offsetMax = new Vector2(width_ / 2, -topOffset_);

			var bg = ui_.AddComponent<Image>();
			bg.color = new Color(0, 0, 0, 0.8f);
			bg.raycastTarget = true;

			SuperController.singleton.AddCanvas(canvas_);

			return true;
		}

		public override void Destroy()
		{
			if (canvas_ != null)
				SuperController.singleton.RemoveCanvas(canvas_);

			if (panel_ != null)
			{
				Object.Destroy(panel_);
				panel_ = null;
			}
		}

		protected override Canvas GetCanvas()
		{
			return canvas_;
		}
	}
}
