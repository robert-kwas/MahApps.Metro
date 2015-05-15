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
        private const string DefaultValidatePasswordWatermark = "Retype Your New Password...";
        private const string DefaultRecoveryEmailWatermark = "New Email...";
        private const string DefaultRecoveryEmailMessage = "Change Recovery Email. (Optional)";
        private const string DefaultValidateEmailWatermark = "Retype Your New Email...";
        private const Visibility DefaultCurrentPasswordVisibility = Visibility.Visible;
        private const Visibility DefaultRecoveryEmailVisibility = Visibility.Collapsed;
        private const Visibility DefaultRecoveryEmailMessageVisibility = Visibility.Collapsed;
        private const bool DefaultEnablePasswordPreview = false;

        public ChangePasswordDialogSettings()
        {
            CurrentPasswordWatermark = DefaultCurrentPasswordWatermark;
            NewPasswordWatermark = DefaultNewPasswordWatermark;
            ValidatePasswordWatermark = DefaultValidatePasswordWatermark;
            CurrentPasswordVisibility = DefaultCurrentPasswordVisibility;
            RecoveryEmailWatermark = DefaultRecoveryEmailWatermark;
            RecoveryEmailVisibility = DefaultRecoveryEmailVisibility;
            RecoveryEmailMessage = DefaultRecoveryEmailMessage;
            RecoveryEmailMessageVisibility = DefaultRecoveryEmailMessageVisibility;
            ValidateEmailWatermark = DefaultValidateEmailWatermark;
            AffirmativeButtonText = "Set Password";
            NegativeButtonText = "Cancel";
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string InitialPassword { get; set; }

        public string CurrentPasswordWatermark { get; set; }

        public string NewPasswordWatermark { get; set; }

        public string ValidatePasswordWatermark { get; set; }

        public string RecoveryEmailWatermark { get; set; }

        public string ValidateEmailWatermark { get; set; }

        public string RecoveryEmailMessage { get; set; }

        public Visibility RecoveryEmailMessageVisibility { get; set; }

        public Visibility RecoveryEmailVisibility { get; set; }

        public Visibility ValidateEmailVisibility { get; set; }

        public Visibility CurrentPasswordVisibility { get; set; }

        public bool EnablePasswordPreview { get; set; }
    }

    public class ChangePasswordDialogData
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ValidatePassword { get; set; }
        public string RecoveryEmail { get; set; }
        public string RecoveryEmailMessage { get; set; }
        public string ValidateEmail { get; set; }
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
            ValidatePasswordWatermark = settings.ValidatePasswordWatermark;
            CurrentPasswordVisibility = settings.CurrentPasswordVisibility;
            RecoveryEmailVisibility = settings.RecoveryEmailVisibility;
            RecoveryEmailWatermark = settings.RecoveryEmailWatermark;
            RecoveryEmailMessageVisibility = settings.RecoveryEmailMessageVisibility;
            RecoveryEmailMessage = settings.RecoveryEmailMessage;
            ValidateEmailWatermark = settings.ValidateEmailWatermark;
            ValidateEmailVisibility = settings.RecoveryEmailVisibility;

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
                    tcs.TrySetResult(new ChangePasswordDialogData { CurrentPassword = PART_PasswordBox.Password, NewPassword = PART_PasswordBox2.Password, ValidatePassword = PART_PasswordBox3.Password, RecoveryEmail = PART_TextBox.Text, ValidateEmail = PART_TextBox2.Text });
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

                tcs.TrySetResult(new ChangePasswordDialogData { CurrentPassword = PART_PasswordBox.Password, NewPassword = PART_PasswordBox2.Password, ValidatePassword = PART_PasswordBox3.Password, RecoveryEmail = PART_TextBox.Text, ValidateEmail = PART_TextBox2.Text });

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_PasswordBox.KeyDown += affirmativeKeyHandler;
            PART_PasswordBox2.KeyDown += affirmativeKeyHandler;
            PART_PasswordBox3.KeyDown += affirmativeKeyHandler;
            PART_TextBox.KeyDown += affirmativeKeyHandler;
            PART_TextBox2.KeyDown += affirmativeKeyHandler;

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
                    PART_TextBlock.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CurrentPasswordProperty = DependencyProperty.Register("CurrentPassword", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CurrentPasswordWatermarkProperty = DependencyProperty.Register("CurrentPasswordWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty NewPasswordProperty = DependencyProperty.Register("NewPassword", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty NewPasswordWatermarkProperty = DependencyProperty.Register("NewPasswordWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ValidatePasswordProperty = DependencyProperty.Register("ValidatePassword", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ValidatePasswordWatermarkProperty = DependencyProperty.Register("ValidatePasswordWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty CurrentPasswordVisibilityProperty = DependencyProperty.Register("CurrentPasswordVisibility", typeof(Visibility), typeof(ChangePasswordDialog), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty RecoveryEmailVisibilityProperty = DependencyProperty.Register("RecoveryEmailVisibility", typeof(Visibility), typeof(ChangePasswordDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty RecoveryEmailProperty = DependencyProperty.Register("RecoveryEmail", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RecoveryEmailWatermarkProperty = DependencyProperty.Register("RecoveryEmailWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RecoveryEmailMessageProperty = DependencyProperty.Register("RecoveryEmailMessage", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RecoveryEmailMessageVisibilityProperty = DependencyProperty.Register("RecoveryEmailMessageVisibility", typeof(Visibility), typeof(ChangePasswordDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ValidateEmailProperty = DependencyProperty.Register("ValidateEmailMessage", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ValidateEmailWatermarkProperty = DependencyProperty.Register("ValidateEmailWatermark", typeof(string), typeof(ChangePasswordDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ValidateEmailVisibilityProperty = DependencyProperty.Register("ValidateEmailVisibility", typeof(Visibility), typeof(ChangePasswordDialog), new PropertyMetadata(Visibility.Collapsed));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string RecoveryEmailMessage
        {
            get { return (string)GetValue(RecoveryEmailMessageProperty); }
            set { SetValue(RecoveryEmailMessageProperty, value); }
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

        public string ValidatePassword
        {
            get { return (string)GetValue(ValidatePasswordWatermarkProperty); }
            set { SetValue(ValidatePasswordWatermarkProperty, value); }
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

        public string ValidatePasswordWatermark
        {
            get { return (string)GetValue(ValidatePasswordWatermarkProperty); }
            set { SetValue(ValidatePasswordWatermarkProperty, value); }
        }

        public string RecoveryEmailWatermark
        {
            get { return (string)GetValue(RecoveryEmailWatermarkProperty); }
            set { SetValue(RecoveryEmailWatermarkProperty, value); }
        }

        public string ValidateEmailWatermark
        {
            get { return (string)GetValue(ValidateEmailWatermarkProperty); }
            set { SetValue(ValidateEmailWatermarkProperty, value); }
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

        public string RecoveryEmail
        {
            get { return (string)GetValue(RecoveryEmailProperty); }
            set { SetValue(RecoveryEmailProperty, value); }
        }

        public string ValidateEmail
        {
            get { return (string)GetValue(ValidateEmailProperty); }
            set { SetValue(ValidateEmailProperty, value); }
        }

        public Visibility CurrentPasswordVisibility
        {
            get { return (Visibility)GetValue(CurrentPasswordVisibilityProperty); }
            set { SetValue(CurrentPasswordVisibilityProperty, value); }
        }

        public Visibility RecoveryEmailVisibility
        {
            get { return (Visibility)GetValue(RecoveryEmailVisibilityProperty); }
            set { SetValue(RecoveryEmailVisibilityProperty, value); }
        }

        public Visibility RecoveryEmailMessageVisibility
        {
            get { return (Visibility)GetValue(RecoveryEmailMessageVisibilityProperty); }
            set { SetValue(RecoveryEmailMessageVisibilityProperty, value); }
        }

        public Visibility ValidateEmailVisibility
        {
            get { return (Visibility)GetValue(ValidateEmailVisibilityProperty); }
            set { SetValue(ValidateEmailVisibilityProperty, value); }
        }
    }
}