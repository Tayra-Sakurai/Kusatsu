# Accessibility and Design Preservation Guidelines

For this project (C# .NET / Windows App SDK / WinUI 3), our top priority is to build an application that offers a consistent user experience and complies fully with Windows accessibility features.

To achieve this, our core principle is to **preserve and respect WinUI's default appearance, built-in themes, and platform accessibility capabilities**.

---

## 1. Core Principles

1. **Respect Defaults**
   - Use standard WinUI 3 controls (`Button`, `TextBox`, `ListView`, etc.) and their default styles whenever possible.
   - Avoid creating heavy custom styles or templates (`ControlTemplate`) that alter control appearances unless strictly necessary.
2. **Inherit Platform Features**
   - Ensure Windows accessibility settings (High Contrast themes, text scaling, visual effects, etc.) function seamlessly out of the box without extra code.
3. **Design Consistency**
   - Maintain the Fluent Design System look and feel to provide a UI that harmonizes with the Windows OS.

---

## 2. Visual Design & Styling Rules

### 2.1 Colors & Themes
- **No Hardcoded Colors**: Do not hardcode HEX or RGB color values (e.g., `#FFFFFFFF`). Always use **`ThemeResource`** or **`StaticResource`** markup extensions (e.g., `{ThemeResource SystemControlForegroundBaseHighBrush}`).
- **Maintain Contrast**: Rely on the default theme palettes to ensure proper contrast in Light, Dark, and High Contrast themes.

### 2.2 Typography
- Do not override font families or sizes on individual controls. Instead, use standard WinUI text styles (e.g., `SubtitleTextBlockStyle`, `BodyTextBlockStyle`). This ensures automatic response to system-level text scaling settings.

### 2.3 Focus Visuals
- Do not remove or reduce the visibility of keyboard focus rings.
- Always retain the default WinUI focus visual behavior for accessibility and keyboard navigation.

---

## 3. Internationalization (i18n) & UI Strings

To ensure accessibility for users in different regions and languages, **all user-facing UI text must be localizable**:

- **Use Resource Files (.resw)**: Do not hardcode UI text strings directly in XAML or C# code.
- **XAML Localization**: Assign an `x:Uid` attribute to UI elements in XAML (e.g., `<Button x:Uid="SubmitButton" />`) and manage strings in string resource files (`Resources.resw`).
- **Accessible Names & Tooltips**: Set `AutomationProperties.Name` via resource files for elements that require accessibility labels, ensuring translated descriptions for screen reader users.

---

## 4. Control Selection & Customization Criteria

- **Limit Custom Controls**: Avoid creating custom UI components from scratch, as they require implementing custom UI Automation peers (`AutomationPeer`). Rely on combining standard WinUI controls instead.
- **Interactive Elements**: Always use standard interactive controls (such as `Button` or `HyperlinkButton`) so that screen readers (e.g., Narrator) can properly recognize element roles and behaviors.

---

## 5. Implementation Checklist

Before submitting a Pull Request or completing a code review, verify the following:

- [ ] Are all UI text strings localized using resource files (`.resw`) and `x:Uid` attributes (no hardcoded text in XAML/C#)?
- [ ] Are colors referenced via `ThemeResource` instead of hardcoded HEX/RGB values?
- [ ] Have unnecessary control template or style overrides been avoided?
- [ ] Does the UI render correctly in Dark Mode and Windows High Contrast themes?
- [ ] Is the focus indicator clearly visible when navigating via keyboard (Tab / Arrow keys)?
- [ ] Do screen readers (e.g., Narrator) correctly announce control roles, names, and states?