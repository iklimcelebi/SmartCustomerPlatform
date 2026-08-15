import {
  BrowserRouter,
  Navigate,
  NavLink,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from "react-router-dom";

import DashboardPage from "./pages/DashboardPage";
import CategoryPage from "./pages/CategoryPage";
import CreateTicketPage from "./pages/CreateTicketPage";
import DepartmentPage from "./pages/DepartmentPage";
import LoginPage from "./pages/LoginPage";
import ProjectionPage from "./pages/ProjectionPage";
import SlaDashboardPage from "./pages/SlaDashboardPage";
import TicketDetailPage from "./pages/TicketDetailPage";
import TicketListPage from "./pages/TicketListPage";
import CategoryDetailPage from "./pages/CategoryDetailPage";
import "./App.css";
import ProtectedRoute from "./components/common/ProtectedRoute";


function AppLayout() {
  const location = useLocation();
  const navigate = useNavigate();
  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("role");
    localStorage.removeItem("name");

    navigate("/login", { replace: true });
  };

  const isDashboard =
    location.pathname === "/" ||
    location.pathname === "/dashboard";

  const getPageName = () => {
    if (location.pathname.startsWith("/tickets/create")) {
      return "Yeni Talep";
    }

    if (location.pathname.startsWith("/tickets/")) {
      return "Talep Detayı";
    }

    if (location.pathname === "/tickets") {
      return "Talepler";
    }

    if (location.pathname === "/departments") {
      return "Departmanlar";
    }

    if (location.pathname.startsWith("/categories/")) {
      return "Kategori Detayı";
    }

    if (location.pathname === "/categories") {
      return "Kategoriler";
    }

    if (location.pathname === "/sla-dashboard") {
      return "SLA Dashboard";
    }

    if (location.pathname === "/projection") {
      return "Projection";
    }

    return "Dashboard";
  };

  return (
    <div className="app">
      <header className="topbar">
        <div className="topbar-left">
          <button
            className="mobile-menu-button"
            onClick={() =>
              document.body.classList.toggle("sidebar-open")
            }
            aria-label="Menüyü aç"
          >
            ☰
          </button>

          <div
            className="brand"
            onClick={() => navigate("/categories")}
          >
            <div className="brand-mark">S</div>

            <div>
              <div className="brand-name">
                SmartCustomer
              </div>

              <div className="brand-subtitle">
                Customer Support Platform
              </div>
            </div>
          </div>
        </div>

        <div className="topbar-right">
          <div className="topbar-page-name">
            {getPageName()}
          </div>

          <div className="topbar-divider" />

          <div className="topbar-user">
            <div className="avatar">
              {(
                localStorage.getItem("name") ||
                localStorage.getItem("username") ||
                "AD"
              )
                .split(" ")
                .map((part) => part[0])
                .join("")
                .slice(0, 2)
                .toUpperCase()}
            </div>

            <div className="user-info">
              <strong>
                {localStorage.getItem("name") ||
                  localStorage.getItem("username") ||
                  "Admin"}
              </strong>

              <small>
                {localStorage.getItem("role") || "Admin"}
              </small>
            </div>

            <button
              type="button"
              className="logout-button"
              onClick={handleLogout}
            >
              Çıkış
            </button>
          </div>


        </div>
      </header>

      <div className="layout">
        <aside className="sidebar">
          <div className="sidebar-section">
            <div className="sidebar-title">
              GENEL
            </div>

            <NavLink
              to="/"
              end
              className={({ isActive }) =>
                `nav-button ${
                  isActive ? "active" : ""
                }`
              }
            >
              <span className="nav-icon">⌂</span>
              <span>Genel Bakış</span>
            </NavLink>

            <NavLink
              to="/tickets"
              className={({ isActive }) =>
                `nav-button ${
                  isActive ? "active" : ""
                }`
              }
            >
              <span className="nav-icon">▤</span>
              <span>Talepler</span>
            </NavLink>
          </div>

          <div className="sidebar-section">
            <div className="sidebar-title">
              YÖNETİM
            </div>

            <NavLink
              to="/departments"
              className={({ isActive }) =>
                `nav-button ${
                  isActive ? "active" : ""
                }`
              }
            >
              <span className="nav-icon">▥</span>
              <span>Departmanlar</span>
            </NavLink>

            <NavLink
              to="/categories"
              className={({ isActive }) =>
                `nav-button ${
                  isActive ? "active" : ""
                }`
              }
            >
              <span className="nav-icon">◇</span>
              <span>Kategoriler</span>
            </NavLink>
          </div>

          <div className="sidebar-section">
            <div className="sidebar-title">
              RAPORLAMA
            </div>

            <NavLink
              to="/sla-dashboard"
              className={({ isActive }) =>
                `nav-button ${
                  isActive ? "active" : ""
                }`
              }
            >
              <span className="nav-icon">◴</span>
              <span>SLA Dashboard</span>
            </NavLink>

            <NavLink
              to="/projection"
              className={({ isActive }) =>
                `nav-button ${
                  isActive ? "active" : ""
                }`
              }
            >
              <span className="nav-icon">▦</span>
              <span>Projection</span>
            </NavLink>
          </div>

          <div className="sidebar-bottom">
            <div className="system-status">
              <span className="online-dot" />

              <div>
                <strong>Sistem Aktif</strong>
                <small>Servisler normal çalışıyor</small>
              </div>
            </div>
          </div>
        </aside>

        <main className="main-content">
          {!isDashboard && (
            <div className="global-back-bar">
              <button
                className="back-button"
                onClick={() => navigate(-1)}
              >
                ← Geri
              </button>

              <span>
                {getPageName()}
              </span>
            </div>
          )}

          <Routes>
            <Route
              path="/"
              element={<DashboardPage />}
            />

            <Route
              path="/dashboard"
              element={<DashboardPage />}
            />

            <Route
              path="/tickets"
              element={<TicketListPage />}
            />

            <Route
              path="/tickets/create"
              element={<CreateTicketPage />}
            />

            <Route
              path="/tickets/:ticketId"
              element={<TicketDetailPage />}
            />

            <Route
              path="/departments"
              element={<DepartmentPage />}
            />

            <Route
              path="/categories"
              element={<CategoryPage />}
            />

            <Route
              path="/categories/:categoryId"
              element={<CategoryDetailPage />}
            />

            <Route
              path="/projection"
              element={<ProjectionPage />}
            />

            <Route
              path="/sla-dashboard"
              element={<SlaDashboardPage />}
            />

            <Route
              path="*"
              element={<Navigate to="/" replace />}
            />
          </Routes>
        </main>
      </div>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/login"
          element={<LoginPage />}
        />

        <Route element={<ProtectedRoute />}>
          <Route
            path="/*"
            element={<AppLayout />}
          />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
