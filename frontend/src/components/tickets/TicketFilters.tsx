type Props = {
  search: string;
  status: string;
  priority: string;
  categoryId: string;

  onSearchChange: (value: string) => void;
  onStatusChange: (value: string) => void;
  onPriorityChange: (value: string) => void;
  onCategoryChange: (value: string) => void;

  categories: Array<{
    id: string;
    name: string;
  }>;
};

export default function TicketFilters({
  search,
  status,
  priority,
  categoryId,
  onSearchChange,
  onStatusChange,
  onPriorityChange,
  onCategoryChange,
  categories,
}: Props) {
  return (
    <div className="card form-card">
      <div className="card-header">
        <div>
          <h2>Filtreler</h2>

          <p>
            Talep listesini filtreleyin.
          </p>
        </div>
      </div>

      <div className="form-grid">
        <div className="form-group">
          <label>Arama</label>

          <input
            type="text"
            value={search}
            onChange={(e) =>
              onSearchChange(e.target.value)
            }
            placeholder="Talep ara..."
          />
        </div>

        <div className="form-group">
          <label>Durum</label>

          <select
            value={status}
            onChange={(e) =>
              onStatusChange(e.target.value)
            }
          >
            <option value="">
              Tüm durumlar
            </option>

            <option value="Open">
              Açık
            </option>

            <option value="InProgress">
              Devam ediyor
            </option>

            <option value="WaitingForCustomer">
              Müşteri bekleniyor
            </option>

            <option value="Resolved">
              Çözüldü
            </option>

            <option value="Closed">
              Kapalı
            </option>
          </select>
        </div>

        <div className="form-group">
          <label>Öncelik</label>

          <select
            value={priority}
            onChange={(e) =>
              onPriorityChange(e.target.value)
            }
          >
            <option value="">
              Tüm öncelikler
            </option>

            <option value="1">
              Düşük
            </option>

            <option value="2">
              Orta
            </option>

            <option value="3">
              Yüksek
            </option>

            <option value="4">
              Kritik
            </option>
          </select>
        </div>

        <div className="form-group">
          <label>Kategori</label>

          <select
            value={categoryId}
            onChange={(e) =>
              onCategoryChange(e.target.value)
            }
          >
            <option value="">
              Tüm kategoriler
            </option>

            {categories.map((category) => (
              <option
                key={category.id}
                value={category.id}
              >
                {category.name}
              </option>
            ))}
          </select>
        </div>
      </div>
    </div>
  );
}