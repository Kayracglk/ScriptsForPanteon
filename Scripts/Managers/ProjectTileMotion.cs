using UnityEngine;
/// <summary>
/// Calculate the all values for projectile motion
/// </summary>
public struct ProjectTileMotion
{
    public Vector2 speed;

    public float time;
    public float distance;
    public float height;

    public ProjectTileMotion CalculateProjectTileMotionValues(float m_speed, float m_angle)
    {
        float gravity = -10;

        float radian = m_angle * (Mathf.PI) / 180;

        speed.x = m_speed * Mathf.Abs(Mathf.Cos(radian));
        speed.y = m_speed * Mathf.Abs(Mathf.Sin(radian));
        //Debug.Log(speed.y);

        float time1 = 2 * Mathf.Abs(speed.y / gravity);
        float time2 = (speed.y + Mathf.Sqrt(Mathf.Pow(speed.y, 2) + 20)) / 10;
        //Debug.Log(time1 + " " + time2);
        if (time2 <= 0) time2 = (speed.y - Mathf.Sqrt(Mathf.Pow(speed.y, 2) + 20)) / 10;

        time = time1 + time2;
        distance = time * speed.x;
        height = speed.y * speed.y / (2 * Mathf.Abs(gravity));
        //Debug.Log(time +  " Distance  : " + distance);


        return this;
    }

    public ProjectTileMotion CalculateProjectTileMotionValues(Vector2 m_speed)
    {
        float gravity = -10;

        speed.x = m_speed.x;
        speed.y = m_speed.y;

        time = 2 * speed.y / Mathf.Abs(gravity);
        distance = speed.x * time;
        height = speed.y * speed.y / (2 * Mathf.Abs(gravity));

        return this;
    }
}