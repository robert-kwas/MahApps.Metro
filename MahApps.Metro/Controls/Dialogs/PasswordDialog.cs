using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public class PasswordDialogSettings : MetroDialogSettings
    {
        private const string DefaultPasswordWatermark = "Password...";
        private const Visibility DefaultNegativeButtonVisibility = Visibility.Collapsed;
        private const bool DefaultEnablePasswordPreview = false;

        public PasswordDialogSettings()
        {
            PasswordWatermark = DefaultPasswordWatermark;
            NegativeButtonVisibility = DefaultNegativeButtonVisibility;
            AffirmativeButtonText = "OK";
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string PasswordWatermark { get; set; }

        public Visibility NegativeButtonVisibility { get; set; }

        public bool EnablePasswordPreview { get; set; }
    }

    public class PasswordDialogData
    {
        public string Password { get; set; }
    }

    public partial class PasswordDialog : BaseMetroDialog
    {
        internal PasswordDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal PasswordDialog(MetroWindow parentWindow, PasswordDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
            PasswordWatermark = settings.PasswordWatermark;
            NegativeButtonButtonVisibility = settings.NegativeButtonVisibility;
            if (settings.EnablePasswordPreview)
            {
                object resource = Application.Current.FindResource("Win8MetroPasswordBox");
                if (resource != null && resource.GetType() == typeof(Style))
                {
                    PART_TextBox.Style = (Style)resource;
                }
            }
        }

        internal Task<PasswordDialogData> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                PART_TextBox.Focus();
            }));

            TaskCompletionSource<PasswordDialogData> tcs = new TaskCompletionSource<PasswordDialogData>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = () =>
            {
                PART_TextBox.KeyDown -= affirmativeKeyHandler;

                this.KeyDown -= escapeKeyHandler;

                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
            };

            escapeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            negativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            affirmativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();
                    tcs.TrySetResult(new PasswordDialogData { Password = PART_TextBox.Password });
                }
            };

            negativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(null);

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(new PasswordDialogData { Password = PART_TextBox.Password });

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_TextBox.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.Style = this.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(PasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(PasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(PasswordDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(PasswordDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty NegativeButtonButtonVisibilityProperty = DependencyProperty.Register("NegativeButtonButtonVisibility", typeof(Visibility), typeof(PasswordDialog), new PropertyMetadata(Visibility.Collapsed));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public string PasswordWatermark
        {
            get { return (string)GetValue(PasswordWatermarkProperty); }
            set { SetValue(PasswordWatermarkProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)GetValue(AffirmativeButtonTextProperty); }
            set { SetValue(AffirmativeButtonTextProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }

        public Visibility NegativeButtonButtonVisibility
        {
            get { return (Visibility)GetValue(NegativeButtonButtonVisibilityProperty); }
            set { SetValue(NegativeButtonButtonVisibilityProperty, value); }
        }
    }
}
