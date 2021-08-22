[System.Serializable]
public class SettingsData
{
    public int language = 0;
    
    public float sensitivity = 1f;
    public bool smoothScrolling = true;

    public int graphics = 1;
    public int resolution = -1;
    public int brightness = 5;
    public bool fullScreen = true;
    public bool vSync = true;

    public int music = 10;
    public int ui = 10;

    public SettingsData(int _resolution)
    {
        resolution = _resolution;
        sensitivity = 1f;
        smoothScrolling = false;
        graphics = 1;
        music = 5;
        ui = 5;
}
}