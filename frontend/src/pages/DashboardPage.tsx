import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";

import Loading from "../components/common/Loading";
import ErrorMessage from "../components/common/ErrorMessage";

import { getTickets } from "../services/ticketService";
import { getDepartments } from "../services/departmentService";
import { getCategories } from "../services/categoryService";

type DashboardTicket = {
  id: string;
  ticketNumber?: string;
  subject?: string;
  title?: string;
  description?: string;

  status: string | number;
  priority: number;

  customerName?: string;
  categoryName?: string;

  createdAt?: string;
  occurredOn?: string;
};

type DashboardDepartment = {
  id: string;
  name: string;
  code?: string;
  status: number;
};

type DashboardCategory = {
  id: string;
  name: string;
};

export default function DashboardPage() {
  const navigate = useNavigate();

  const [tickets, setTickets] = useState<DashboardTicket[]>([]);
  const [departments, setDepartments] = useState<
    DashboardDepartment[]
  >([]);
  const [categories, setCategories] = useState<
    DashboardCategory[]
  >([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // ============================================================
  // DASHBOARD DATA
  // ============================================================

  const loadDashboard = async () => {
    try {
      setLoading(true);
      setError("");

      const [
        ticketResponse,
        departmentResponse,
        categoryResponse,
      ] = await Promise.all([
        getTickets({}),
        getDepartments(),
        getCategories(),
      ]);

      const ticketResult = ticketResponse as
        | DashboardTicket[]
        | { items?: DashboardTicket[] }
        | null;

      const ticketList = Array.isArray(ticketResult)
        ? ticketResult
        : ticketResult?.items ?? [];

      setTickets(ticketList);
      setDepartments(departmentResponse);
      setCategories(categoryResponse);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Dashboard verileri yüklenemedi."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDashboard();
  }, []);

  // ============================================================
  // HELPERS
  // ============================================================

  const normalize = (value: unknown) =>
    String(value ?? "")
      .toLowerCase()
      .replace(/[\s_-]/g, "");

  // ============================================================
  // STATISTICS
  // ============================================================

  const statistics = useMemo(() => {
    const open = tickets.filter(
      (ticket) => normalize(ticket.status) === "open"
    ).length;

    const inProgress = tickets.filter(
      (ticket) =>
        normalize(ticket.status) === "inprogress"
    ).length;

    const waiting = tickets.filter(
      (ticket) =>
        normalize(ticket.status) === "waitingforcustomer"
    ).length;

    const resolved = tickets.filter((ticket) => {
      const status = normalize(ticket.status);

      return (
        status === "resolved" ||
        status === "closed"
      );
    }).length;

    // Backend enum:
    //
    // Low      = 1
    // Medium   = 2
    // High     = 3
    // Critical = 4

    const low = tickets.filter(
      (ticket) => ticket.priority === 1
    ).length;

    const medium = tickets.filter(
      (ticket) => ticket.priority === 2
    ).length;

    const high = tickets.filter(
      (ticket) => ticket.priority === 3
    ).length;

    const critical = tickets.filter(
      (ticket) => ticket.priority === 4
    ).length;

    return {
      total: tickets.length,
      open,
      inProgress,
      waiting,
      resolved,
      critical,
      high,
      medium,
      low,
    };
  }, [tickets]);

  // ============================================================
  // RECENT TICKETS
  // ============================================================

  const recentTickets = useMemo(() => {
    return [...tickets]
      .sort((a, b) => {
        const dateAValue =
          a.createdAt ?? a.occurredOn;

        const dateBValue =
          b.createdAt ?? b.occurredOn;

        if (!dateAValue) return 1;
        if (!dateBValue) return -1;

        const dateA = new Date(dateAValue).getTime();
        const dateB = new Date(dateBValue).getTime();

        if (
          Number.isNaN(dateA) ||
          Number.isNaN(dateB)
        ) {
          return 0;
        }

        return dateB - dateA;
      })
      .slice(0, 6);
  }, [tickets]);

  // ============================================================
  // PRIORITY HELPERS
  // ============================================================

  const getPriorityLabel = (priority: number) => {
    switch (priority) {
      case 1:
        return "Düşük";

      case 2:
        return "Orta";

      case 3:
        return "Yüksek";

      case 4:
        return "Kritik";

      default:
        return "-";
    }
  };

  const getPriorityClass = (priority: number) => {
    switch (priority) {
      case 1:
        return "dashboard-priority low";

      case 2:
        return "dashboard-priority medium";

      case 3:
        return "dashboard-priority high";

      case 4:
        return "dashboard-priority critical";

      default:
        return "dashboard-priority low";
    }
  };

  // ============================================================
  // STATUS HELPERS
  // ============================================================

  const getStatusClass = (
    status: string | number
  ) => {
    const normalized = normalize(status);

    if (
      normalized === "resolved" ||
      normalized === "closed"
    ) {
      return "dashboard-status resolved";
    }

    if (normalized === "inprogress") {
      return "dashboard-status progress";
    }

    if (
      normalized === "waitingforcustomer"
    ) {
      return "dashboard-status waiting";
    }

    return "dashboard-status open";
  };

  const getStatusLabel = (
    status: string | number
  ) => {
    const normalized = normalize(status);

    switch (normalized) {
      case "open":
        return "Açık";

      case "inprogress":
        return "İşlemde";

      case "waitingforcustomer":
        return "Müşteri Bekleniyor";

      case "resolved":
        return "Çözüldü";

      case "closed":
        return "Kapalı";

      default:
        return String(status);
    }
  };

  // ============================================================
  // DATE
  // ============================================================

  const formatDate = (date?: string) => {
    if (!date) return "-";

    const parsed = new Date(date);

    if (
      Number.isNaN(parsed.getTime()) ||
      parsed.getFullYear() < 2000
    ) {
      return "-";
    }

    return parsed.toLocaleDateString(
      "tr-TR",
      {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
      }
    );
  };

  // ============================================================
  // TICKET TITLE
  // ============================================================

  const getTicketTitle = (
    ticket: DashboardTicket
  ) => {
    if (
      ticket.subject &&
      ticket.subject.trim()
    ) {
      return ticket.subject;
    }

    if (
      ticket.title &&
      ticket.title.trim()
    ) {
      return ticket.title;
    }

    if (ticket.ticketNumber) {
      return `Talep #${ticket.ticketNumber}`;
    }

    return `Talep #${ticket.id.slice(0, 8)}`;
  };

  // ============================================================
  // LOADING
  // ============================================================

  if (loading) {
    return (
      <div className="dashboard-loading">
        <Loading message="Dashboard yükleniyor..." />
      </div>
    );
  }

  // ============================================================
  // RENDER
  // ============================================================

  return (
    <div className="dashboard-page">
      <div className="container">

        <ErrorMessage message={error} />

        {/* HEADER */}

        <div className="dashboard-header">
          <div>
            <div className="breadcrumb">
              Ana Sayfa
            </div>

            <h1>Genel Bakış</h1>

            <p>
              Müşteri destek operasyonlarınızın
              güncel durumunu tek ekrandan
              takip edin.
            </p>
          </div>

          <button
            className="primary-button dashboard-create-button"
            onClick={() =>
              navigate("/tickets/create")
            }
          >
            <span>＋</span>
            Yeni Talep
          </button>
        </div>

        {/* STATISTICS */}

        <div className="dashboard-stat-grid">

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-top">
              <span className="dashboard-stat-label">
                TOPLAM TALEP
              </span>

              <div className="dashboard-stat-icon purple">
                #
              </div>
            </div>

            <strong>
              {statistics.total}
            </strong>

            <span className="dashboard-stat-description">
              Sistemdeki tüm destek talepleri
            </span>
          </div>

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-top">
              <span className="dashboard-stat-label">
                AÇIK
              </span>

              <div className="dashboard-stat-icon blue">
                ○
              </div>
            </div>

            <strong>
              {statistics.open}
            </strong>

            <span className="dashboard-stat-description">
              İşleme alınmayı bekleyen
            </span>
          </div>

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-top">
              <span className="dashboard-stat-label">
                İŞLEMDE
              </span>

              <div className="dashboard-stat-icon orange">
                ◌
              </div>
            </div>

            <strong>
              {statistics.inProgress}
            </strong>

            <span className="dashboard-stat-description">
              Ekip tarafından üzerinde çalışılan
            </span>
          </div>

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-top">
              <span className="dashboard-stat-label">
                ÇÖZÜLEN
              </span>

              <div className="dashboard-stat-icon green">
                ✓
              </div>
            </div>

            <strong>
              {statistics.resolved}
            </strong>

            <span className="dashboard-stat-description">
              Başarıyla sonuçlandırılan
            </span>
          </div>

        </div>

        {/* MAIN GRID */}

        <div className="dashboard-main-grid">

          {/* RECENT TICKETS */}

          <div className="card dashboard-recent-card">

            <div className="card-header dashboard-card-header">
              <div>
                <h2>Son Talepler</h2>

                <p>
                  Sisteme eklenen son destek
                  talepleri
                </p>
              </div>

              <button
                className="text-button"
                onClick={() =>
                  navigate("/tickets")
                }
              >
                Tümünü Gör →
              </button>
            </div>

            {recentTickets.length === 0 ? (
              <div className="empty">
                Henüz destek talebi bulunmuyor.
              </div>
            ) : (
              <div className="dashboard-ticket-list">

                {recentTickets.map(
                  (ticket) => (
                    <div
                      className="dashboard-ticket-row"
                      key={ticket.id}
                      onClick={() =>
                        navigate(
                          `/tickets/${ticket.id}`
                        )
                      }
                    >

                      <div className="dashboard-ticket-main">

                        <div className="dashboard-ticket-title">
                          {getTicketTitle(
                            ticket
                          )}
                        </div>

                        <div className="dashboard-ticket-meta">
                          <span>
                            #
                            {ticket.ticketNumber ??
                              ticket.id.slice(0, 8)}
                          </span>

                          <span>•</span>

                          <span>
                            {ticket.customerName ||
                              "Müşteri bilgisi yok"}
                          </span>
                        </div>

                      </div>

                      <div className="dashboard-ticket-category">
                        {ticket.categoryName ||
                          "Genel"}
                      </div>

                      <span
                        className={getPriorityClass(
                          ticket.priority
                        )}
                      >
                        {getPriorityLabel(
                          ticket.priority
                        )}
                      </span>

                      <span
                        className={getStatusClass(
                          ticket.status
                        )}
                      >
                        {getStatusLabel(
                          ticket.status
                        )}
                      </span>

                      <div className="dashboard-ticket-date">
                        {formatDate(
                          ticket.createdAt ??
                            ticket.occurredOn
                        )}
                      </div>

                      <button
                        className="dashboard-view-button"
                        onClick={(event) => {
                          event.stopPropagation();

                          navigate(
                            `/tickets/${ticket.id}`
                          );
                        }}
                      >
                        →
                      </button>

                    </div>
                  )
                )}

              </div>
            )}

          </div>

          {/* RIGHT COLUMN */}

          <div className="dashboard-side-column">

            {/* PRIORITY */}

            <div className="card dashboard-overview-card">

              <div className="card-header dashboard-card-header">
                <div>
                  <h2>Talep Özeti</h2>

                  <p>
                    Öncelik dağılımı
                  </p>
                </div>
              </div>

              <div className="dashboard-priority-list">

                <div className="dashboard-priority-row">
                  <span>
                    <i className="priority-dot critical-dot" />
                    Kritik
                  </span>

                  <strong>
                    {statistics.critical}
                  </strong>
                </div>

                <div className="dashboard-priority-row">
                  <span>
                    <i className="priority-dot high-dot" />
                    Yüksek
                  </span>

                  <strong>
                    {statistics.high}
                  </strong>
                </div>

                <div className="dashboard-priority-row">
                  <span>
                    <i className="priority-dot medium-dot" />
                    Orta
                  </span>

                  <strong>
                    {statistics.medium}
                  </strong>
                </div>

                <div className="dashboard-priority-row">
                  <span>
                    <i className="priority-dot low-dot" />
                    Düşük
                  </span>

                  <strong>
                    {statistics.low}
                  </strong>
                </div>

              </div>

              <div className="dashboard-alert">
                <div className="dashboard-alert-icon">
                  !
                </div>

                <div>
                  <strong>
                    Öncelikli Talepler
                  </strong>

                  <span>
                    {statistics.critical} kritik
                    ve{" "}
                    {statistics.high} yüksek
                    öncelikli talep var.
                  </span>

                  <button
                    onClick={() =>
                      navigate("/tickets")
                    }
                  >
                    Talepleri İncele →
                  </button>
                </div>
              </div>

            </div>

            {/* QUICK ACTIONS */}

            <div className="card dashboard-quick-card">

              <div className="card-header dashboard-card-header">
                <div>
                  <h2>Hızlı İşlemler</h2>

                  <p>
                    Sık kullanılan ekranlar
                  </p>
                </div>
              </div>

              <div className="dashboard-quick-actions">

                <button
                  onClick={() =>
                    navigate(
                      "/tickets/create"
                    )
                  }
                >
                  <span className="quick-icon blue">
                    +
                  </span>

                  <span>
                    <strong>
                      Yeni Talep
                    </strong>

                    <small>
                      Destek talebi oluştur
                    </small>
                  </span>

                  <b>→</b>
                </button>

                <button
                  onClick={() =>
                    navigate(
                      "/departments"
                    )
                  }
                >
                  <span className="quick-icon purple">
                    ▥
                  </span>

                  <span>
                    <strong>
                      Departmanlar
                    </strong>

                    <small>
                      Departmanları yönet
                    </small>
                  </span>

                  <b>→</b>
                </button>

                <button
                  onClick={() =>
                    navigate(
                      "/categories"
                    )
                  }
                >
                  <span className="quick-icon orange">
                    ◇
                  </span>

                  <span>
                    <strong>
                      Kategoriler
                    </strong>

                    <small>
                      Talep kategorilerini yönet
                    </small>
                  </span>

                  <b>→</b>
                </button>

                <button
                  onClick={() =>
                    navigate(
                      "/sla-dashboard"
                    )
                  }
                >
                  <span className="quick-icon green">
                    ◴
                  </span>

                  <span>
                    <strong>
                      SLA Dashboard
                    </strong>

                    <small>
                      SLA performansını takip et
                    </small>
                  </span>

                  <b>→</b>
                </button>

              </div>

            </div>

          </div>

        </div>

        {/* BOTTOM MODULES */}

        <div className="dashboard-section-title">
          <div>
            <h2>Platform Yönetimi</h2>

            <p>
              Destek operasyonunun temel bileşenlerini
              yönetin.
            </p>
          </div>
        </div>

        <div className="dashboard-bottom-grid">

          <div
            className="dashboard-module-card"
            onClick={() =>
              navigate("/departments")
            }
          >
            <div className="dashboard-module-icon purple">
              {departments.length}
            </div>

            <div className="dashboard-module-content">
              <span>DEPARTMANLAR</span>

              <strong>
                Destek organizasyonu
              </strong>

              <small>
                Departmanları ve durumlarını yönetin
              </small>
            </div>

            <button>→</button>
          </div>

          <div
            className="dashboard-module-card"
            onClick={() =>
              navigate("/categories")
            }
          >
            <div className="dashboard-module-icon orange">
              {categories.length}
            </div>

            <div className="dashboard-module-content">
              <span>KATEGORİLER</span>

              <strong>
                Talep sınıflandırması
              </strong>

              <small>
                Kategori ve alt kategorileri yönetin
              </small>
            </div>

            <button>→</button>
          </div>

          <div
            className="dashboard-module-card"
            onClick={() =>
              navigate("/sla-dashboard")
            }
          >
            <div className="dashboard-module-icon green">
              SLA
            </div>

            <div className="dashboard-module-content">
              <span>SERVİS SEVİYESİ</span>

              <strong>
                SLA performansı
              </strong>

              <small>
                Hizmet seviyesi göstergelerini takip edin
              </small>
            </div>

            <button>→</button>
          </div>

        </div>

      </div>
    </div>
  );
}