﻿namespace Aimtec.SDK.Menu.Components
{
    using System;

    using Aimtec.SDK.Menu.Theme;
    using Aimtec.SDK.Util;
    using Newtonsoft.Json;
    using System.IO;
    using System.Reflection;

    /// <summary>
    ///     Class MenuKeybind. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Menu.MenuComponent" />
    /// <seealso cref="bool" />
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class MenuKeyBind : MenuComponent, IReturns<bool>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        /// </summary>
        /// <param name="internalName">The internal name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="key">The key.</param>
        /// <param name="keybindType">Type of the keybind.</param>
        /// <param name="shared">Whether this item is shared across instances</param>
        public MenuKeyBind(string internalName, string displayName, Keys key, KeybindType keybindType, bool shared = false)
        {
            this.InternalName = internalName;
            this.DisplayName = displayName;
            this.Key = key;
            this.KeybindType = keybindType;

            this.Shared = shared;

            var callingAssembly = Assembly.GetCallingAssembly();

            this.CallingAssemblyName = $"{callingAssembly.GetName().Name}.{callingAssembly.GetType().GUID}";

            this.LoadValue();
        }

        [JsonConstructor]
        private MenuKeyBind()
        {
        }

        #endregion

        #region Public Properties

        internal override string Serialized => JsonConvert.SerializeObject(this, Formatting.Indented);


        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonProperty(Order = 3)]
        public new bool Value { get; set; }


        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [JsonProperty(Order = 4)]
        public Keys Key { get; set; }

        /// <summary>
        ///     Gets or sets the type of the keybind.
        /// </summary>
        /// <value>The type of the keybind.</value>
        public KeybindType KeybindType { get; set; }


        /// <summary>
        ///     Gets whether this menu componenet is enabled if applicable
        /// </summary>
        /// <value>The value.</value>
        public new bool Enabled => Value;


        /// <summary>
        /// Gets or sets a value indicating whether the key is being set.
        /// </summary>
        /// <value><c>true</c> if the key is being set; otherwise, <c>false</c>.</value>
        internal bool KeyIsBeingSet { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the render manager.
        /// </summary>
        /// <returns>Aimtec.SDK.Menu.Theme.IRenderManager.</returns>
        public override IRenderManager GetRenderManager()
        {
            return MenuManager.Instance.Theme.BuildMenuKeyBindRenderer(this);
        }

        /// <summary>
        ///     An application-defined function that processes messages sent to a window.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="wparam">Additional message information.</param>
        /// <param name="lparam">Additional message information.</param>
        public override void WndProc(uint message, uint wparam, int lparam)
        {
            if (this.Visible)
            {
                var x = lparam & 0xffff;
                var y = lparam >> 16;

                if (message == (ulong)WindowsMessages.WM_LBUTTONDOWN)
                {
                    if (!this.KeyIsBeingSet && this.GetBounds(this.Position).Contains(x, y))
                    {
                        if (!MenuManager.Instance.Theme.GetMenuBoolControlBounds(this.Position).Contains(x, y))
                        {
                            this.KeyIsBeingSet = true;
                        }

                        else
                        {
                            UpdateValue(!this.Value);
                        }
                    }
                }

                if (this.KeyIsBeingSet && message == (ulong)WindowsMessages.WM_KEYUP)
                {
                    this.UpdateKey((Keys)wparam);
                    this.KeyIsBeingSet = false;
                }
            }

            if (wparam != (ulong)this.Key || this.KeyIsBeingSet)
            {
                return;
            }

            if (this.KeybindType == KeybindType.Press)
            {
                if (message == (ulong)WindowsMessages.WM_KEYDOWN)
                {
                    this.UpdateValue(true);
                }
                else if (message == (ulong)WindowsMessages.WM_KEYUP)
                {
                    this.UpdateValue(false);
                }
            }

            else if (message == (ulong)WindowsMessages.WM_KEYUP)
            {
                this.UpdateValue(!this.Value);
            }
        }



        #endregion

        #region Methods

        /// <summary>
        ///     Updates the value of the KeyBind, saves the new value and fires the value changed event
        /// </summary>
        private void UpdateValue(bool newVal)
        {
            var oldClone = new MenuKeyBind { Value = this.Value, InternalName = this.InternalName, DisplayName = this.DisplayName, Key = this.Key, KeybindType = this.KeybindType };

            this.Value = newVal;

            if (this.KeybindType == KeybindType.Toggle)
            {
                this.SaveValue();
            }

            this.FireOnValueChanged(this, new ValueChangedArgs(oldClone, this));
        }

        private void UpdateKey(Keys key)
        {
            var oldClone = new MenuKeyBind { Value = this.Value, InternalName = this.InternalName, DisplayName = this.DisplayName, Key = this.Key, KeybindType = this.KeybindType };

            this.Key = key;

            this.SaveValue();

            this.FireOnValueChanged(this, new ValueChangedArgs(oldClone, this));
        }

        /// <summary>
        ///    Loads the value from the file for this component
        /// </summary>
        internal override void LoadValue()
        {
            if (File.Exists(this.ConfigPath))
            {
                var read = File.ReadAllText(this.ConfigPath);

                var sValue = JsonConvert.DeserializeObject<MenuKeyBind>(read);

                if (sValue?.InternalName != null)
                {
                    this.Value = sValue.Value;
                    this.Key = sValue.Key;
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Enum KeybindType
    /// </summary>
    public enum KeybindType
    {
        /// <summary>
        ///     Press key bind.
        /// </summary>
        Press,

        /// <summary>
        ///     Toggle key bind.
        /// </summary>
        Toggle
    }
}