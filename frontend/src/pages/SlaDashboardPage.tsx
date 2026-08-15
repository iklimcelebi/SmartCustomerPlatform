import { useEffect, useState } from "react";
import Loading from "../components/common/Loading";
import ErrorMessage from "../components/common/ErrorMessage";
import { getSlaDashboard } from "../services/ticketService";

type SlaItem = {
  id?: string;
  ticketId?: string;
  title?: string;
  status?: string;
  remainingMinutes?: number;
  isBreached?: boolean;
};

const STATUS_LABELS: Record<string, string> = {
  Open: "Açık",
  InProgress: "Devam ediyor",
  WaitingForCustomer: "Müşteri bekleniyor",
  Resolved: "Çözüldü",
  Closed: "Kapalı",
};

function getStatusLabel(status?: string) {
  if (!status) {
    return "-";
  }

  return STATUS_LABELS[status] ?? status;
}

function getSlaLabel(item: SlaItem) {
  if (item.isBreached) {
    if (
      item.remainingMinutes !== undefined &&
      item.remainingMinutes > 0
    ) {
      return "İhlal var";
    }

    return "İhlal";
  }

  return "Uygun";
}

function getSlaClass(item: SlaItem) {
  if (item.isBreached) {
    return "passive";
  }

  return "active";
}

function formatRemainingTime(
  remainingMinutes?: number
) {
  if (
    remainingMinutes === undefined ||
    remainingMinutes === null
  ) {
    return "-";
  }

  if (remainingMinutes <= 0) {
    return "Süre doldu";
  }

  if (remainingMinutes < 60) {
    return `${remainingMinutes} dk`;
  }

  const hours = Math.floor(
    remainingMinutes / 60
  );

  const minutes =
    remainingMinutes % 60;

  if (minutes === 0) {
    return `${hours} sa`;
  }

  return `${hours} sa ${minutes} dk`;
}

export default function SlaDashboardPage() {
  const [items, setItems] =
    useState<SlaItem[]>([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        setLoading(true);
        setError("");

        const data =
          await getSlaDashboard();

        const result =
          data as
            | SlaItem[]
            | { items?: SlaItem[] }
            | null;

        setItems(
          Array.isArray(result)
            ? result
            : result?.items ?? []
        );
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "SLA verileri yüklenemedi."
        );
      } finally {
        setLoading(false);
      }
    };

    loadDashboard();
  }, []);

  return (
    <div className="container">
      <div className="page-header">
        <div>
          <div className="breadcrumb">
            Raporlar <span>/</span> SLA
          </div>

          <h1>SLA Dashboard</h1>

          <p>
            Taleplerin SLA durumlarını ve
            ihlal risklerini takip edin.
          </p>
        </div>
      </div>

      <ErrorMessage message={error} />

      <div className="card">
        <div className="card-header">
          <div>
            <h2>SLA Durumu</h2>

            <p>
              Aktif taleplerin SLA bilgileri.
            </p>
          </div>
        </div>

        <div
          className="muted"
          style={{
            marginBottom: 20,
            lineHeight: 1.6,
          }}
        >
          <strong>SLA durumu nasıl okunur?</strong>
          <br />

          <span>
            <strong>Uygun:</strong> Talebin SLA
            süreleri içerisinde olduğu anlamına gelir.
          </span>

          <br />

          <span>
            <strong>İhlal var:</strong> Talebin
            SLA kurallarından en az biri ihlal
            edilmiştir. Kalan çözüm süresi
            bulunması, diğer SLA süresinin
            hâlâ devam ettiği anlamına gelebilir.
          </span>
        </div>

        {loading ? (
          <Loading
            message="SLA verileri yükleniyor..."
          />
        ) : items.length === 0 ? (
          <div className="empty">
            SLA kaydı bulunmuyor.
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Talep</th>
                  <th>Durum</th>
                  <th>Kalan Çözüm Süresi</th>
                  <th>SLA Durumu</th>
                </tr>
              </thead>

              <tbody>
                {items.map(
                  (item, index) => (
                    <tr
                      key={
                        item.id ??
                        item.ticketId ??
                        index
                      }
                    >
                      <td>
                        <strong>
                          {item.title ||
                            `Talep #${
                              item.ticketId ??
                              "-"
                            }`}
                        </strong>
                      </td>

                      <td>
                        {getStatusLabel(
                          item.status
                        )}
                      </td>

                      <td>
                        {formatRemainingTime(
                          item.remainingMinutes
                        )}
                      </td>

                      <td>
                        <span
                          className={`status ${getSlaClass(
                            item
                          )}`}
                        >
                          {getSlaLabel(item)}
                        </span>
                      </td>
                    </tr>
                  )
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}