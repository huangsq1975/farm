using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 在 WeChat 小游戏 WebGL 环境中使用微信/系统字体渲染中文。
/// Font.CreateDynamicFontFromOSFont 在 WebGL 里通过浏览器 CSS font-family
/// 请求字体，WeChat 的 WebView 会使用设备系统字体（支持中文显示）。
/// </summary>
public static class SystemFontManager
{
    private static Font _font;

    /// <summary>
    /// 初始化系统字体并应用到场景内所有已激活的 TextMesh / UI.Text 组件。
    /// 在 Manager.Awake() 末尾调用，确保场景对象已完成实例化。
    /// </summary>
    public static void Init()
    {
        // 按优先顺序尝试字体名称；WebGL 中会通过浏览器 CSS font-family 解析
        // iOS WeChat: PingFang SC（苹方）; Android WeChat: Noto Sans CJK SC / DroidSansFallback
        // "sans-serif" 作为通用兜底，确保始终有系统字体可用
        _font = Font.CreateDynamicFontFromOSFont(
            new[] { "PingFang SC", "STHeitiSC-Medium", "Noto Sans CJK SC", "DroidSansFallback", "sans-serif" },
            40
        );

        foreach (TextMesh tm in Object.FindObjectsOfType<TextMesh>())
        {
            ApplyToTextMesh(tm);
        }

        foreach (Text t in Object.FindObjectsOfType<Text>())
        {
            t.font = _font;
        }
    }

    public static Font GetFont()
    {
        return _font;
    }

    /// <summary>
    /// 对单个 TextMesh 应用系统字体（同时更新 Renderer 材质，否则字体不生效）。
    /// </summary>
    public static void ApplyToTextMesh(TextMesh tm)
    {
        if (_font == null) return;
        tm.font = _font;
        Renderer r = tm.GetComponent<Renderer>();
        if (r != null) r.sharedMaterial = _font.material;
    }

    /// <summary>
    /// 对动态实例化的 GameObject 及其所有子对象应用系统字体。
    /// 在 Resources.Load + Instantiate 之后调用。
    /// </summary>
    public static void ApplyToGameObject(GameObject go)
    {
        if (_font == null) return;
        foreach (TextMesh tm in go.GetComponentsInChildren<TextMesh>(true))
        {
            ApplyToTextMesh(tm);
        }
        foreach (Text t in go.GetComponentsInChildren<Text>(true))
        {
            t.font = _font;
        }
    }
}
