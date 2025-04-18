﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Definitions.English;
using Microsoft.Recognizers.Text.DateTime.Utilities;

namespace Microsoft.Recognizers.Text.DateTime.English
{
    public class EnglishTimePeriodParserConfiguration : BaseDateTimeOptionsConfiguration, ITimePeriodParserConfiguration
    {
        public EnglishTimePeriodParserConfiguration(ICommonDateTimeParserConfiguration config)
            : base(config)
        {
            TimeExtractor = config.TimeExtractor;
            IntegerExtractor = config.IntegerExtractor;
            TimeParser = config.TimeParser;
            TimeZoneParser = config.TimeZoneParser;

            PureNumberFromToRegex = EnglishTimePeriodExtractorConfiguration.PureNumFromTo;
            PureNumberBetweenAndRegex = EnglishTimePeriodExtractorConfiguration.PureNumBetweenAnd;
            SpecificTimeFromToRegex = EnglishTimePeriodExtractorConfiguration.SpecificTimeFromTo;
            SpecificTimeBetweenAndRegex = EnglishTimePeriodExtractorConfiguration.SpecificTimeBetweenAnd;
            TimeOfDayRegex = EnglishTimePeriodExtractorConfiguration.TimeOfDayRegex;
            GeneralEndingRegex = EnglishTimePeriodExtractorConfiguration.GeneralEndingRegex;
            TillRegex = EnglishTimePeriodExtractorConfiguration.TillRegex;
            TimePeriodWithDurationRegex = EnglishTimePeriodExtractorConfiguration.TimePeriodWithDurationRegex;
            DurationParser = config.DurationParser;
            DurationExtractor = config.DurationExtractor;

            Numbers = config.Numbers;
            UtilityConfiguration = config.UtilityConfiguration;
        }

        public IDateTimeExtractor TimeExtractor { get; }

        public IDateTimeParser TimeParser { get; }

        public IExtractor IntegerExtractor { get; }

        public IDateTimeParser TimeZoneParser { get; }

        public IDateTimeParser DurationParser { get; }

        public IDateTimeExtractor DurationExtractor { get; }

        public Regex SpecificTimeFromToRegex { get; }

        public Regex SpecificTimeBetweenAndRegex { get; }

        public Regex PureNumberFromToRegex { get; }

        public Regex PureNumberBetweenAndRegex { get; }

        public Regex TimeOfDayRegex { get; }

        public Regex GeneralEndingRegex { get; }

        public Regex TillRegex { get; }

        public Regex TimePeriodWithDurationRegex { get; }

        public IImmutableDictionary<string, int> Numbers { get; }

        public IDateTimeUtilityConfiguration UtilityConfiguration { get; }

        public bool GetMatchedTimeRange(string text, out string timex, out int beginHour, out int endHour, out int endMin)
        {
            var trimmedText = text.Trim();
            if (trimmedText.EndsWith("s", StringComparison.Ordinal))
            {
                trimmedText = trimmedText.Substring(0, trimmedText.Length - 1);
            }

            beginHour = 0;
            endHour = 0;
            endMin = 0;

            var timeOfDay = string.Empty;
            if (DateTimeDefinitions.MorningTermList.Any(o => trimmedText.EndsWith(o, StringComparison.Ordinal)))
            {
                timeOfDay = Constants.Morning;
            }
            else if (DateTimeDefinitions.AfternoonTermList.Any(o => trimmedText.EndsWith(o, StringComparison.Ordinal)))
            {
                timeOfDay = Constants.Afternoon;
            }
            else if (DateTimeDefinitions.EveningTermList.Any(o => trimmedText.EndsWith(o, StringComparison.Ordinal)))
            {
                timeOfDay = Constants.Evening;
            }
            else if (DateTimeDefinitions.DaytimeTermList.Any(o => trimmedText.EndsWith(o, StringComparison.Ordinal)))
            {
                timeOfDay = Constants.Daytime;
            }
            else if (DateTimeDefinitions.NighttimeTermList.Any(o => trimmedText.EndsWith(o, StringComparison.Ordinal)))
            {
                timeOfDay = Constants.Nighttime;
            }
            else if (DateTimeDefinitions.NightTermList.Any(o => trimmedText.EndsWith(o, StringComparison.Ordinal)))
            {
                timeOfDay = Constants.Night;
            }
            else if (DateTimeDefinitions.BusinessHourSplitStrings.All(o => trimmedText.Contains(o)))
            {
                timeOfDay = Constants.BusinessHour;
            }
            else if (DateTimeDefinitions.MealtimeBreakfastTermList.Any(o => trimmedText.Contains(o)))
            {
                timeOfDay = Constants.MealtimeBreakfast;
            }
            else if (DateTimeDefinitions.MealtimeBrunchTermList.Any(o => trimmedText.Contains(o)))
            {
                timeOfDay = Constants.MealtimeBrunch;
            }
            else if (DateTimeDefinitions.MealtimeLunchTermList.Any(o => trimmedText.Contains(o)))
            {
                timeOfDay = Constants.MealtimeLunch;
            }
            else if (DateTimeDefinitions.MealtimeDinnerTermList.Any(o => trimmedText.Contains(o)))
            {
                timeOfDay = Constants.MealtimeDinner;
            }
            else
            {
                timex = null;
                return false;
            }

            var parseResult = TimexUtility.ResolveTimeOfDay(timeOfDay);
            timex = parseResult.Timex;
            beginHour = parseResult.BeginHour;
            endHour = parseResult.EndHour;
            endMin = parseResult.EndMin;

            if ((Options & DateTimeOptions.TasksMode) != 0)
            {
                beginHour = 0;
                endHour = 0;
                endMin = 0;
                parseResult = TasksModeProcessing.TasksModeResolveTimeOfDay(timeOfDay);
                timex = parseResult.Timex;
                beginHour = parseResult.BeginHour;
                endHour = parseResult.EndHour;
                endMin = parseResult.EndMin;
            }

            return true;
        }
    }
}
