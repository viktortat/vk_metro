﻿using System.Windows.Media.Imaging;

namespace VK_Metro
{
    using System.Windows;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using VK_Metro.Utilities;
using System.Collections.Generic;

    public partial class App : Application
    {
        private static VK_API vk = null;

        public static List<BitmapImage> Attachments = new List<BitmapImage>();

        private static LongPollListener lpListener;

        /// <summary>
        /// VK
        /// </summary>
        /// <returns>Объект VK</returns>
        public static VK_API VK
        {
            get
            {
                // Отложить создание модели представления до необходимости
                if (vk == null)
                    vk = new VK_API();

                return vk;
            }
        }

        public static LongPollListener LpListener
        {
            get
            {
                // Отложить создание модели представления до необходимости
                return lpListener ?? (lpListener = new LongPollListener(vk));
            }
        }

        private static VK_Metro.Models.MainPageModel mainpagedata = null;

        /// <summary>
        /// MainPageModel
        /// </summary>
        /// <returns>Объект MainPageModel</returns>
        public static VK_Metro.Models.MainPageModel MainPageData
        {
            get
            {
                // Отложить создание модели представления до необходимости
                if (mainpagedata == null)
                    mainpagedata = new VK_Metro.Models.MainPageModel();

                return mainpagedata;
            }
        }

        /// <summary>
        /// Обеспечивает быстрый доступ к корневому кадру приложения телефона.
        /// </summary>
        /// <returns>Корневой кадр приложения для телефона.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Конструктор объекта приложения.
        /// </summary>
        public App()
        {
            // Глобальный обработчик неперехваченных исключений. 
            UnhandledException += Application_UnhandledException;

            // Стандартная инициализация Silverlight
            InitializeComponent();

            // Инициализация телефона
            InitializePhoneApplication();

            // Отображение сведений о профиле графики во время отладки.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Отображение текущих счетчиков частоты смены кадров
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Отображение областей приложения, перерисовываемых в каждом кадре.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Включение режима визуализации анализа нерабочего кода 
                // для отображения областей страницы, переданных в GPU, с цветным наложением.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Отключите обнаружение простоя приложения, установив для свойства UserIdleDetectionMode
                // объекта PhoneApplicationService приложения значение Disabled.
                // Внимание! Используйте только в режиме отладки. Приложение, в котором отключено обнаружение бездействия пользователя, будет продолжать работать
                // и потреблять энергию батареи, когда телефон не будет использоваться.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // Код для выполнения при запуске приложения (например, из меню "Пуск")
        // Этот код не будет выполняться при повторной активации приложения
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            var t = VK_Metro.Views.Authorization.BackgroundProperty;
        }

        // Код для выполнения при активации приложения (переводится в основной режим)
        // Этот код не будет выполняться при первом запуске приложения
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            var t = VK_Metro.Views.Authorization.BackgroundProperty;
            var currentPage = ((PhoneApplicationFrame)Application.Current.RootVisual).Content;
        }

        // Код для выполнения при деактивации приложения (отправляется в фоновый режим)
        // Этот код не будет выполняться при закрытии приложения
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Код для выполнения при закрытии приложения (например, при нажатии пользователем кнопки "Назад")
        // Этот код не будет выполняться при деактивации приложения
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Убедитесь, что здесь сохраняется необходимое состояние приложения.
        }

        // Код для выполнения в случае ошибки навигации
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Ошибка навигации; перейти в отладчик
                System.Diagnostics.Debugger.Break();
            }
        }

        // Код для выполнения на необработанных исключениях
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Произошло необработанное исключение; перейти в отладчик
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Инициализация приложения телефона

        // Избегайте двойной инициализации
        private bool phoneApplicationInitialized = false;

        // Не добавляйте в этот метод дополнительный код
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Создайте кадр, но не задавайте для него значение RootVisual; это позволит
            // экрану-заставке оставаться активным, пока приложение не будет готово для визуализации.
            //RootFrame = new PhoneApplicationFrame();
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Обработка сбоев навигации
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Убедитесь, что инициализация не выполняется повторно
            phoneApplicationInitialized = true;
        }

        // Не добавляйте в этот метод дополнительный код
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Задайте корневой визуальный элемент для визуализации приложения
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Удалите этот обработчик, т.к. он больше не нужен
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}