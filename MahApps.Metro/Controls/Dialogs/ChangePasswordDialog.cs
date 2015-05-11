using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public class ChangePasswordDialogSettings : MetroDialogSettings
    {
        private const string DefaultCurrentPasswordWatermark = "Current Password...";
        private const string DefaultNewPasswordWatermark = "New Password...";
        private const string DefaultValidationPasswordWatermark = "Retype Your New Password...";
        private const Visibility DefaultCurrentPasswordVisibility = Visibility.Visible;
        private const bool DefaultEnablePasswordPreview = false;

        public ChangePasswordDialogSettings()
        {
            CurrentPasswordWatermark = DefaultCurrentPasswordWatermark;
            NewPasswordWatermark = DefaultNewPasswordWatermark;
            ValidationPasswordWatermark = DefaultValidationPasswordWatermark;
            CurrentPasswordVisibility = DefaultCurrentPasswordVisibility;
            AffirmativeButtonText = "Set Password";
            NegativeButtonText = "Cancel";
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string InitialPassword { get; set; }

        public string CurrentPasswordWatermark { get; set; }

        public string NewPasswordWatermark { get; set; }

        public string ValidationPasswordWatermark { get; set; }

        public Visibility CurrentPasswordVisibility { get; set; }

        public bool EnablePasswordPreview { get; set; }
    }

    public class ChangePasswordDialogData
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ValidationPassword { get; set; }
    }

    public partial class ChangePasswordDialog : BaseMetroDialog
    {
        internal ChangePasswordDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal ChangePasswordDialog(MetroWindow parentWindow, ChangePasswordDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
            CurrentPassword = settings.InitialPassword;
            CurrentPasswordWatermark = settings.CurrentPasswordWatermark;
            NewPasswordWatermark = settings.NewPasswordWatermark;
            ValidationPasswordWatermark = settings.ValidationPasswordWatermark;
            CurrentPasswordVisibility = settings.CurrentPasswordVisibility;
            
            if (settings.EnablePasswordPreview)
            {
                object resource = Application.Current.FindResource("Win8MetroPasswordBox");
                if (resource != null && resource.GetType() == typeof(Style))
                {
                    PART_PasswordBox.Style = (Style)resource;
                    PART_PasswordBox2.Style = (Style)resource;
                    PART_PasswordBox3.Style = (Style)resource;
                }
            }
        }

        internal Task<ChangePasswordDialogData> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                PART_PasswordBox.Focus();
            }));

            TaskCompletionSource<ChangePasswordDialogData> tcs = new TaskCompletionSource<ChangePasswordDialogData>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = () =>
            {
                PART_PasswordBox3.KeyDown -= affirmativeKeyHandler;

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
                    tcs.TrySetResult(new ChangePasswordDialogData { CurrentPassword = PART_PasswordBox.Password, NewPassword = PART_PasswordBox2.Password, ValidationPassword = PART_PasswordBox3.Password });
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

                tcs.TrySetResult(new ChangePasswordDialogData { CurrentPassword = PART_PasswordBox.Password, NewPassword = PART_PasswordBox2.Password, ValidationPassword = PART_PasswordBox3.Password });

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_PasswordBox.KeyDown += affirmativeKeyHandler;
            PART_PasswordBox2.KeyDown += affirmativeKeyHandler;
            PART_PasswordBox3.KeyDown += affirmativeKeyHandler;

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
                    PART_PasswordBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_PasswordBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_PasswordBox3.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CurrentPasswordProperty = DependencyProperty.Register("CurrentPassword", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CurrentPasswordWatermarkProperty = DependencyProperty.Register("CurrentPasswordWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty NewPasswordProperty = DependencyProperty.Register("NewPassword", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty NewPasswordWatermarkProperty = DependencyProperty.Register("NewPasswordWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ValidationPasswordProperty = DependencyProperty.Register("ValidationPassword", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ValidationPasswordWatermarkProperty = DependencyProperty.Register("ValidationPasswordWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty CurrentPasswordVisibilityProperty = DependencyProperty.Register("CurrentPasswordVisibility", typeof(Visibility), typeof(ChangePasswordDialog), new PropertyMetadata(Visibility.Collapsed));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string CurrentPassword
        {
            get { return (string)GetValue(CurrentPasswordProperty); }
            set { SetValue(CurrentPasswordProperty, value); }
        }

        public string NewPassword
        {
            get { return (string)GetValue(NewPasswordProperty); }
            set { SetValue(NewPasswordProperty, value); }
        }

        public string ValidationPassword
        {
            get { return (string)GetValue(ValidationPasswordWatermarkProperty); }
            set { SetValue(ValidationPasswordWatermarkProperty, value); }
        }

        public string CurrentPasswordWatermark
        {
            get { return (string)GetValue(CurrentPasswordWatermarkProperty); }
            set { SetValue(CurrentPasswordWatermarkProperty, value); }
        }

        public string NewPasswordWatermark
        {
            get { return (string)GetValue(NewPasswordWatermarkProperty); }
            set { SetValue(NewPasswordWatermarkProperty, value); }
        }

        public string ValidationPasswordWatermark
        {
            get { return (string)GetValue(ValidationPasswordWatermarkProperty); }
            set { SetValue(ValidationPasswordWatermarkProperty, value); }
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

        public Visibility CurrentPasswordVisibility
        {
            get { return (Visibility)GetValue(CurrentPasswordVisibilityProperty); }
            set { SetValue(CurrentPasswordVisibilityProperty, value); }
        }
    }
}