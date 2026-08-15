import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import Loading from "../components/common/Loading";
import ErrorMessage from "../components/common/ErrorMessage";
import TicketFilters from "../components/tickets/TicketFilters";

import { getTickets } from "../services/ticketService";
import { getCategories } from "../services/categoryService";

import type { TicketSearchResult } from "../types";

type Category = {
  id: string;
  name: string;
};

export default function TicketListPage() {
  const navigate = useNavigate();

  const [tickets, setTickets] =
    useState<TicketSearchResult[]>([]);

  const [categories, setCategories] =
    useState<Category[]>([]);

  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [priority, setPriority] = useState("");
  const [categoryId, setCategoryId] =
    useState("");

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const loadTickets = async () => {
    try {
      setLoading(true);
      setError("");

      const data = await getTickets({
        search: search || undefined,
        status: status || undefined,
        priority: priority || undefined,
        categoryId:
          categoryId || undefined,
      });

      setTickets(
        Array.isArray(data)
          ? data
          : []
      );
    } catch (err) {
      console.error(err);

      setError(
        err instanceof Error
          ? err.message
          : "Talepler yüklenemedi."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const data = await getCategories();

        setCategories(data);
      } catch (err) {
        console.error(err);

        setError(
          err instanceof Error
            ? err.message
            : "Kategoriler yüklenemedi."
        );
      }
    };

    loadCategories();
  }, []);

  useEffect(() => {
    loadTickets();
  }, [
    search,
    status,
    priority,
    categoryId,
  ]);

  const getPriorityLabel = (
    value: string
  ) => {
    switch (String(value)) {
      case "1":
      case "Low":
        return "Düşük";

      case "2":
      case "Medium":
        return "Orta";

      case "3":
      case "High":
        return "Yüksek";

      case "4":
      case "Critical":
        return "Kritik";

      default:
        return value || "-";
    }
  };

  const getStatusLabel = (
    value: string
  ) => {
    switch (value) {
      case "Open":
        return "Açık";

      case "InProgress":
        return "Devam ediyor";

      case "WaitingForCustomer":
        return "Müşteri bekleniyor";

      case "Resolved":
        return "Çözüldü";

      case "Closed":
        return "Kapalı";

      default:
        return value || "-";
    }
  };

  const getCategoryName = (
    id: string
  ) => {
    const category = categories.find(
      (item) => item.id === id
    );

    return (
      category?.name ||
      id ||
      "-"
    );
  };

  return (
    <div className="container">
      <div className="page-header">
        <div>
          <div className="breadcrumb">
            Ana Sayfa <span>/</span> Talepler
          </div>

          <h1>Talepler</h1>

          <p>
            Müşteri destek taleplerini
            görüntüleyin ve yönetin.
          </p>
        </div>

        <button
          type="button"
          className="primary-button"
          onClick={() =>
            navigate("/tickets/create")
          }
        >
          Yeni Talep
        </button>
      </div>

      <ErrorMessage message={error} />

      <TicketFilters
        search={search}
        status={status}
        priority={priority}
        categoryId={categoryId}
        onSearchChange={setSearch}
        onStatusChange={setStatus}
        onPriorityChange={setPriority}
        onCategoryChange={setCategoryId}
        categories={categories}
      />

      <div className="card">
        <div className="card-header">
          <div>
            <h2>Talep Listesi</h2>

            <p>
              {tickets.length} talep bulundu.
            </p>
          </div>
        </div>

        {loading ? (
          <Loading
            message="Talepler yükleniyor..."
          />
        ) : tickets.length === 0 ? (
          <div className="empty">
            Filtrelere uygun talep bulunamadı.
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Talep</th>
                  <th>Müşteri</th>
                  <th>Kategori</th>
                  <th>Öncelik</th>
                  <th>Durum</th>
                  <th>Tarih</th>
                  <th></th>
                </tr>
              </thead>

              <tbody>
                {tickets.map((ticket) => (
                  <tr
                    key={ticket.ticketId}
                  >
                    <td>
                      <strong>
                        {ticket.subject ||
                          "-"}
                      </strong>

                      <div className="muted">
                        {
                          ticket.ticketNumber
                        }
                      </div>
                    </td>

                    <td>
                      {ticket.customerName ||
                        "-"}
                    </td>

                    <td>
                      {getCategoryName(
                        ticket.categoryId
                      )}
                    </td>

                    <td>
                      <span className="code">
                        {getPriorityLabel(
                          ticket.priority
                        )}
                      </span>
                    </td>

                    <td>
                      <span
                        className={`status ${
                          ticket.status ===
                            "Closed" ||
                          ticket.status ===
                            "Resolved"
                            ? "passive"
                            : "active"
                        }`}
                      >
                        {getStatusLabel(
                          ticket.status
                        )}
                      </span>
                    </td>

                    <td>
                      {ticket.occurredOn
                        ? new Date(
                            ticket.occurredOn
                          ).toLocaleDateString(
                            "tr-TR"
                          )
                        : "-"}
                    </td>

                    <td>
                      <button
                        type="button"
                        className="edit-button"
                        onClick={() =>
                          navigate(
                            `/tickets/${ticket.ticketId}`
                          )
                        }
                      >
                        Görüntüle
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}