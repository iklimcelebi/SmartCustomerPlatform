import { useEffect, useState } from "react";
import Loading from "../components/common/Loading";
import ErrorMessage from "../components/common/ErrorMessage";
import { getProjection } from "../services/ticketService";

type Projection = {
  totalTickets?: number;
  openTickets?: number;
  closedTickets?: number;
  resolvedTickets?: number;
  inProgressTickets?: number;
  averageResolutionTime?: number;
};

const formatDuration = (seconds?: number): string => {
  if (
    seconds === undefined ||
    seconds === null ||
    seconds <= 0
  ) {
    return "0 dk";
  }

  const totalMinutes = Math.round(seconds / 60);

  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;

  if (hours === 0) {
    return `${minutes} dk`;
  }

  if (minutes === 0) {
    return `${hours} sa`;
  }

  return `${hours} sa ${minutes} dk`;
};

export default function ProjectionPage() {
  const [projection, setProjection] =
    useState<Projection | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadProjection = async () => {
      try {
        setLoading(true);
        setError("");

        const data = await getProjection();
        setProjection(data);
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "Projection verileri yüklenemedi."
        );
      } finally {
        setLoading(false);
      }
    };

    loadProjection();
  }, []);

  if (loading) {
    return (
      <div className="container">
        <Loading message="Projection verileri yükleniyor..." />
      </div>
    );
  }

  return (
    <div className="container">
      <div className="page-header">
        <div>
          <div className="breadcrumb">
            Raporlar <span>/</span> Projection
          </div>

          <h1>Talep Projection</h1>

          <p>
            Talep sisteminin projection durumunu ve genel
            istatistiklerini yönetin.
          </p>
        </div>
      </div>

      <ErrorMessage message={error} />

      {!projection ? (
        <div className="card">
          <div className="empty">
            Projection verisi bulunamadı.
          </div>
        </div>
      ) : (
        <div className="two-column">
          <div className="card form-card">
            <div className="card-header">
              <div>
                <h2>Toplam Talepler</h2>
              </div>
            </div>

            <strong style={{ fontSize: "32px" }}>
              {projection.totalTickets ?? 0}
            </strong>
          </div>

          <div className="card form-card">
            <div className="card-header">
              <div>
                <h2>Açık Talepler</h2>
              </div>
            </div>

            <strong style={{ fontSize: "32px" }}>
              {projection.openTickets ?? 0}
            </strong>
          </div>

          <div className="card form-card">
            <div className="card-header">
              <div>
                <h2>Devam Edenler</h2>
              </div>
            </div>

            <strong style={{ fontSize: "32px" }}>
              {projection.inProgressTickets ?? 0}
            </strong>
          </div>

          <div className="card form-card">
            <div className="card-header">
              <div>
                <h2>Çözülen Talepler</h2>
              </div>
            </div>

            <strong style={{ fontSize: "32px" }}>
              {projection.resolvedTickets ?? 0}
            </strong>
          </div>

          <div className="card form-card">
            <div className="card-header">
              <div>
                <h2>Kapanan Talepler</h2>
              </div>
            </div>

            <strong style={{ fontSize: "32px" }}>
              {projection.closedTickets ?? 0}
            </strong>
          </div>

          <div className="card form-card">
            <div className="card-header">
              <div>
                <h2>Ortalama Çözüm Süresi</h2>
              </div>
            </div>

            <strong style={{ fontSize: "32px" }}>
              {formatDuration(
                projection.averageResolutionTime
              )}
            </strong>
          </div>
        </div>
      )}
    </div>
  );
}