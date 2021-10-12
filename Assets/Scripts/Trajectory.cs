using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public BallControl ball;
    CircleCollider2D ballCollider;
    Rigidbody2D ballRigidbody;
    public GameObject ballAtCollision;

    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
        ballCollider = ball.GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        bool drawBallAtCollision = false;
        Vector2 offsetHitPoint = new Vector2();
        RaycastHit2D[] circleCastHit2DArray = Physics2D.CircleCastAll(ballRigidbody.position, 
        ballCollider.radius, ballRigidbody.velocity.normalized);

        foreach (RaycastHit2D circleCastHit2D in circleCastHit2DArray)
        {
            if (circleCastHit2D.collider != null && circleCastHit2D.collider.GetComponent<BallControl>() == null)
            {
                /*Garis lintasan akan digambar dari titik tengah bola saat ini ke titik tengah bola pada saat tumbukan,
                 yaitu sebuah titik yang di-offset dari titik tumbukan berdasar vektor normal titik tersebut sebesar
                 jari-jari bola. */

                // Tentukan titik tumbukan
                Vector2 hitPoint = circleCastHit2D.point;
                // Tentukan normal di titik tumbukan
                Vector2 hitNormal = circleCastHit2D.normal;
                // Tentukan offsetHitPoint, yaitu titik tengah bola pada saat bertumbukan
                offsetHitPoint = hitPoint + hitNormal * ballCollider.radius;
                // Gambar garis lintasan dari titik tengah bola saat ini ke titik tengah bola pada saat bertumbukan
                DottedLine.DottedLine.Instance.DrawDottedLine(ball.transform.position, offsetHitPoint);

                // Kalau bukan sidewall, gambar pantulannya
                if (circleCastHit2D.collider.GetComponent<SideWallScript>() == null)
                {
                    // Hitung vektor datang
                    Vector2 inVector = (offsetHitPoint - ball.TrajectoryOrigin).normalized;
                    // Hitung vektor keluar
                    Vector2 outVector = Vector2.Reflect(inVector, hitNormal);
                    // Hitung dot product dari outVector dan hitNormal. Digunakan supaya garis lintasan ketika 
                    // terjadi tumbukan tidak digambar.
                    float outDot = Vector2.Dot(outVector, hitNormal);
                    if (outDot > -1.0f && outDot < 1.0)
                    {
                        // Gambar lintasan pantulannya
                        DottedLine.DottedLine.Instance.DrawDottedLine(
                            offsetHitPoint,
                            offsetHitPoint + outVector * 10.0f);
                        // Untuk menggambar bola "bayangan" di prediksi titik tumbukan
                        drawBallAtCollision = true;
                    }
                }

                // Jika true, ...
                if (drawBallAtCollision)
                {
                    // Gambar bola "bayangan" di prediksi titik tumbukan
                    ballAtCollision.transform.position = offsetHitPoint;
                    ballAtCollision.SetActive(true);
                }
                else
                {
                    // Sembunyikan bola "bayangan"
                    ballAtCollision.SetActive(false);
                }
                // Hanya gambar lintasan untuk satu titik tumbukan, jadi keluar dari loop
                break;

            }
        }

    }
}