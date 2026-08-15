import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../services/api";
import ErrorMessage from "../components/common/ErrorMessage";

export default function LoginPage() {
  const navigate = useNavigate();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!username.trim() || !password) {
      setError("Kullanıcı adı ve şifre zorunludur.");
      return;
    }

    try {
      setLoading(true);
      setError("");

      const response = await login({
        username: username.trim(),
        password,
      });

      if (response?.token) {
        localStorage.setItem("token", response.token);
        localStorage.setItem(
          "username",
          response.username ?? username.trim()
        );
        localStorage.setItem(
          "role",
          response.role ?? ""
        );
        localStorage.setItem(
          "name",
          response.name ?? ""
        );
      }

      navigate("/dashboard");
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Giriş yapılamadı."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container">
      <div
        className="card form-card"
        style={{
          maxWidth: "460px",
          margin: "70px auto",
        }}
      >
        <div className="card-header">
          <div>
            <h2>Giriş Yap</h2>
            <p>
              SmartCustomerPlatform hesabınıza giriş yapın.
            </p>
          </div>
        </div>

        <ErrorMessage message={error} />

        <form onSubmit={handleSubmit}>
          <div className="form-grid single">
            <div className="form-group">
              <label>Kullanıcı Adı</label>

              <input
                type="text"
                value={username}
                onChange={(event) =>
                  setUsername(event.target.value)
                }
                placeholder="Kullanıcı adınız"
                autoComplete="username"
              />
            </div>

            <div className="form-group">
              <label>Şifre</label>

              <input
                type="password"
                value={password}
                onChange={(event) =>
                  setPassword(event.target.value)
                }
                placeholder="Şifreniz"
                autoComplete="current-password"
              />
            </div>

            <div className="form-actions">
              <button
                type="submit"
                className="primary-button"
                disabled={loading}
              >
                {loading
                  ? "Giriş yapılıyor..."
                  : "Giriş Yap"}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}