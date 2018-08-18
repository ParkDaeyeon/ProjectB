namespace Program.Model.Domain.Code
{
    public static class ConfigCode
    {
        public const long UNDEFINED                                 = -1;
        
        public const long PROCESSABLE_NOTE_START_RATE               = 1000;
        public const long PROCESSABLE_NOTE_END_RATE                 = 1001;

        public const long NOTE_DISAPPEAR_RATE                       = 2000;
        public const long NOTE_AVAILABLE_RATE                       = 2001;
        
        public const long LONG_SUCCEED_DURATION                     = 3000;
        public const long LONG_TIER1_DURATION                       = 3001;

        public const long DANGER_RATE                               = 4000;

        public const long MISS_SFX_WAIT                             = 4010;

        public const long MUSIC_STUTTER_VOLUME                      = 4020;
        public const long MUSIC_STUTTER_DOWN_TIME                   = 4021;
        public const long MUSIC_STUTTER_UP_DELAY                    = 4022;
        public const long MUSIC_STUTTER_UP_TIME                     = 4023;

        public const long AUDIO_SYNC_PRIORITY_LOW                   = 5000;
        public const long AUDIO_SYNC_PRIORITY_HIGH                  = 5001;
        public const long AUDIO_SYNC_PRIORITY_HIGHEST               = 5002;
        public const long AUDIO_SYNC_PRIORITY_RESET_COUNT           = 5003;
        public const long AUDIO_SYNC_MISSING_LIMIT                  = 5004;
        public const long AUDIO_SYNC_RELAX                          = 5005;
        public const long AUDIO_SYNC_RELAX_COUNT                    = 5006;
        public const long AUDIO_VOLUME_PREVIEW                      = 5011;
        public const long AUDIO_VOLUME_BGM                          = 5012;
        public const long AUDIO_TIME_BGM_STOP_FO                    = 5013;
        public const long AUDIO_TIME_BGM_PAUSE_FO                   = 5014;
        public const long AUDIO_TIME_BGM_RESUME_FI                  = 5015;

        public const long DISPLAY_PATTERN_LEVEL_MIN                 = 6000;
        public const long DISPLAY_PATTERN_LEVEL_MAX                 = 6001;

        //public const long PREVIEW_AUDIO_TERM                        = 7001;
        public const long PREVIEW_AUDIO_DELAY                       = 7002;

        public const long MATINEE_MUSICLIST_ANIM_TIME               = 8001;
        public const long LIBRARY_COMPOSERLIST_ANIM_TIME            = 8002;
        public const long LIBRARY_ARCHIVE_PAGE_ANIM_TIME            = 8003;
        public const long LIBRARY_MUSICLIST_ANIM_TIME               = 8004;
        public const long LIBRARY_BIOGRAHPY_SPEED                   = 8005;

        public const long PIANO_ENSEMBLE_DEFAULT_1P                 = 9001;
        public const long PIANO_ENSEMBLE_DEFAULT_2P                 = 9002;

        public const long BUTTON_INPUT_INTERVAL_STANDARD            = 10001;
        public const long BUTTON_INPUT_BEGIN_DELAY_STANDARD         = 10002;
        
        public const long MATINEE_HIDDEN_SONG                       = 11001;

        public const long OPTIMIZE_NUM_OF_CALL_GC                   = 100001;
        public const long OPTIMIZE_WAIT_MS                          = 100002;

        public const long MENU_COLOR                                = 200001;
        public const long MENU_COLOR_SELECT                         = 200002;
        public const long MENU_COLOR_SELECT_FX                      = 200003;
        public const long PIANO_NAME_COLOR_SMALL                    = 200004;
        public const long COMPOSER_YEAR_COLOR                       = 200005;
        public const long IPGUIDE_KEY_COLOR                         = 200006;
    }
}