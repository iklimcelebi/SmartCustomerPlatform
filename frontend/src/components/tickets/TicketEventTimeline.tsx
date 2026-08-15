type EventItem = {
  id: string;
  eventType?: string;
  description?: string;
  createdAt?: string;
  userName?: string;
};

type Props = {
  events: EventItem[];
};

const formatTurkeyDate = (date?: string) => {
  if (!date) {
    return "";
  }

  const normalizedDate =
    date.endsWith("Z") ||
    date.includes("+") ||
    /-\d{2}:\d{2}$/.test(date)
      ? date
      : `${date}Z`;

  return new Date(normalizedDate).toLocaleString(
    "tr-TR",
    {
      timeZone: "Europe/Istanbul",
    }
  );
};

export default function TicketEventTimeline({
  events,
}: Props) {
  return (
    <div className="card">
      <div className="card-header">
        <div>
          <h2>Olay Geçmişi</h2>
          <p>
            Talep üzerinde gerçekleşen işlemler.
          </p>
        </div>
      </div>

      {events.length === 0 ? (
        <div className="empty">
          Henüz olay kaydı bulunmuyor.
        </div>
      ) : (
        <div className="category-list">
          {events.map((event) => (
            <div
              className="category-item"
              key={event.id}
            >
              <div className="category-main">
                <div>
                  <strong>
                    {event.eventType ||
                      "Talep işlemi"}
                  </strong>

                  <p>
                    {event.description ||
                      "İşlem gerçekleştirildi."}
                  </p>

                  {event.userName && (
                    <span className="muted">
                      İşlemi yapan:{" "}
                      {event.userName}
                    </span>
                  )}
                </div>

                {event.createdAt && (
                  <span className="muted">
                    {formatTurkeyDate(
                      event.createdAt
                    )}
                  </span>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}