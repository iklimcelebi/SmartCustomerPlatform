import { useEffect, useState } from "react";

type Props = {
  currentStatus: string;
  currentPriority: number;
  onSave: (
    status: string,
    priority: number
  ) => Promise<void>;
};

const STATUS_OPTIONS = [
  {
    value: "Open",
    label: "Açık",
  },
  {
    value: "InProgress",
    label: "Devam ediyor",
  },
  {
    value: "WaitingForCustomer",
    label: "Müşteri bekleniyor",
  },
  {
    value: "Resolved",
    label: "Çözüldü",
  },
  {
    value: "Closed",
    label: "Kapalı",
  },
];

const PRIORITY_OPTIONS = [
  {
    value: 1,
    label: "Düşük",
  },
  {
    value: 2,
    label: "Orta",
  },
  {
    value: 3,
    label: "Yüksek",
  },
  {
    value: 4,
    label: "Kritik",
  },
];

export default function TicketStatusEditor({
  currentStatus,
  currentPriority,
  onSave,
}: Props) {
  const [status, setStatus] =
    useState(currentStatus);

  const [priority, setPriority] =
    useState(currentPriority);

  const [loading, setLoading] =
    useState(false);

  useEffect(() => {
    setStatus(currentStatus);
  }, [currentStatus]);

  useEffect(() => {
    setPriority(currentPriority);
  }, [currentPriority]);

  const handleSave = async () => {
    try {
      setLoading(true);

      await onSave(
        status,
        priority
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card form-card">
      <div className="card-header">
        <div>
          <h2>Talep Durumu</h2>

          <p>
            Durum ve öncelik bilgisini güncelleyin.
          </p>
        </div>
      </div>

      <div className="form-grid">
        <div className="form-group">
          <label>Durum</label>

          <select
            value={status}
            onChange={(event) =>
              setStatus(event.target.value)
            }
            disabled={loading}
          >
            {STATUS_OPTIONS.map(
              (option) => (
                <option
                  key={option.value}
                  value={option.value}
                >
                  {option.label}
                </option>
              )
            )}
          </select>

          <div
            className="muted"
            style={{
              marginTop: 8,
              lineHeight: 1.5,
            }}
          >
            Durum geçişleri belirli iş kurallarına
            göre yapılır. Açık talepler Devam ediyor
            durumuna alınabilir. Devam ediyor durumundaki
            talepler Müşteri bekleniyor veya Çözüldü
            durumuna geçirilebilir. Müşteri bekleniyor
            durumundaki talepler tekrar Devam ediyor
            durumuna alınabilir. Çözüldü durumundaki
            talepler Kapalı veya tekrar Devam ediyor
            durumuna alınabilir. Kapalı talepler yeniden
            Devam ediyor durumuna alınabilir.
          </div>
        </div>

        <div className="form-group">
          <label>Öncelik</label>

          <select
            value={priority}
            onChange={(event) =>
              setPriority(
                Number(event.target.value)
              )
            }
            disabled={loading}
          >
            {PRIORITY_OPTIONS.map(
              (option) => (
                <option
                  key={option.value}
                  value={option.value}
                >
                  {option.label}
                </option>
              )
            )}
          </select>

          <div
            className="muted"
            style={{
              marginTop: 8,
              lineHeight: 1.5,
            }}
          >
            Öncelik Düşük, Orta, Yüksek veya Kritik
            olarak değiştirilebilir. Kapalı taleplerin
            önceliği değiştirilemez.
          </div>
        </div>
      </div>

      <div className="form-actions">
        <button
          type="button"
          className="primary-button"
          disabled={loading}
          onClick={handleSave}
        >
          {loading
            ? "Kaydediliyor..."
            : "Kaydet"}
        </button>
      </div>
    </div>
  );
}
