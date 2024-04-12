using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    // UI元素
    [SerializeField] private TextMeshProUGUI timeText = null; // 用于显示时间的TextMeshProUGUI组件
    [SerializeField] private TextMeshProUGUI dateText = null; // 用于显示日期的TextMeshProUGUI组件
    [SerializeField] private TextMeshProUGUI seasonText = null; // 用于显示季节的TextMeshProUGUI组件
    [SerializeField] private TextMeshProUGUI yearText = null; // 用于显示年份的TextMeshProUGUI组件

    // 是否使用24小时制
    private bool use24HourFormat = true;

    private void OnEnable()
    {
        // 订阅游戏分钟推进事件，在游戏时间更新时调用UpdateGameTime方法
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        // 取消订阅游戏分钟推进事件
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // 更新时间

        // 将分钟数取整到最近的10分钟
        gameMinute = gameMinute - (gameMinute % 10);

        string time = "";
        string minute;

        // 格式化分钟数，确保显示为两位数
        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }

        // 根据时间格式构建时间字符串
        if (use24HourFormat)
        {
            // 24小时制
            time = gameHour.ToString() + " : " + minute;
        }
        else
        {
            // 12小时制
            string ampm = "";
            int displayHour = gameHour;

            // 根据小时数确定上午还是下午
            if (gameHour >= 12)
            {
                ampm = " pm";
            }
            else
            {
                ampm = " am";
            }

            // 将小时数转换为12小时制
            if (gameHour >= 13)
            {
                displayHour -= 12;
            }
            else if (gameHour == 0)
            {
                displayHour = 12;
            }

            time = displayHour.ToString() + " : " + minute + ampm;
        }

        // 更新UI文本
        timeText.SetText(time); // 更新时间显示
        dateText.SetText(gameDayOfWeek + ". " + gameDay.ToString()); // 更新日期显示
        seasonText.SetText(EnumExtensions.GetEnumDescription(gameSeason)); // 更新季节显示
        yearText.SetText("Year " + gameYear); // 更新年份显示
    }

    /// <summary>
    /// 切换时间显示格式为24小时制或12小时制
    /// </summary>
    public void ToggleTimeFormat()
    {
        use24HourFormat = !use24HourFormat;
    }
}