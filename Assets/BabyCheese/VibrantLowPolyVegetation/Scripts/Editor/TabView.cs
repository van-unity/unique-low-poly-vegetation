using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VibrantLowPolyVegetation.Scripts.Editor.Extensions;

namespace VibrantLowPolyVegetation.Scripts.Editor {
    public class TabViewButtonData {
        public VegetationType VegetationType { get; }
        public Texture IconTexture { get; }

        public TabViewButtonData(VegetationType vegetationType, Texture iconTexture) {
            VegetationType = vegetationType;
            IconTexture = iconTexture;
        }
    }

    public class TabView : VisualElement {
        public static event Action<VegetationType> TabClicked;

        private readonly Dictionary<VegetationType, VisualElement> _tabButtonByVegetationType;

        private VegetationType _current;

        public TabView(TabViewButtonData[] buttonData) {
            _tabButtonByVegetationType = new Dictionary<VegetationType, VisualElement>();
            this.AddStyleSheetFromResources("TabView");
            AddToClassList("tab-view");
            foreach (var tabViewButtonData in buttonData) {
                var button = new VisualElement();
                button.RegisterCallback<ClickEvent>(evt => { OnButtonClicked(tabViewButtonData.VegetationType); });
                button.AddToClassList("tab-button");
                var image = new Image {
                    image = tabViewButtonData.IconTexture
                };
                image.AddToClassList("tab-button-image");
                button.Add(image);
                _tabButtonByVegetationType[tabViewButtonData.VegetationType] = button;
                Add(button);
            }

            _current = VegetationType.PotWithFlowers;
            _tabButtonByVegetationType[_current].AddToClassList("tab-button-selected");
            this.schedule.Execute(() => { TabClicked?.Invoke(_current); }).ExecuteLater(100);
        }

        private void OnButtonClicked(VegetationType vegetationType) {
            _tabButtonByVegetationType[_current].RemoveFromClassList("tab-button-selected");
            _current = vegetationType;
            _tabButtonByVegetationType[_current].AddToClassList("tab-button-selected");
            TabClicked?.Invoke(vegetationType);
        }
    }
}