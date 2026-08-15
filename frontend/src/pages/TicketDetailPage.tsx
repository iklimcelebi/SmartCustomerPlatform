import { useEffect, useState } from "react";
import {
  useNavigate,
  useParams,
} from "react-router-dom";

import TicketComments from "../components/tickets/TicketComments";
import TicketEventTimeline from "../components/tickets/TicketEventTimeline";
import TicketStatusEditor from "../components/tickets/TicketStatusEditor";
import TicketAssignmentModal from "../components/tickets/TicketAssignmentModal";
import TicketTransferModal from "../components/tickets/TicketTransferModal";

import {
  addTicketComment,
  assignTicket,
  changeTicketPriority,
  changeTicketStatus,
  getTicketById,
  getTicketComments,
  getTicketEvents,
  transferTicket,
} from "../services/ticketService";

import { getDepartments } from "../services/departmentService";

import type {
  Ticket,
  TicketComment,
  TicketEvent,
  Department,
} from "../types";

export default function TicketDetailPage() {
  const { ticketId } = useParams();
  const navigate = useNavigate();

  const [ticket, setTicket] =
    useState<Ticket | null>(null);

  const [comments, setComments] =
    useState<TicketComment[]>([]);

  const [events, setEvents] =
    useState<TicketEvent[]>([]);

  const [departments, setDepartments] =
    useState<Department[]>([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [saving, setSaving] =
    useState(false);

  const [
    assignmentModalOpen,
    setAssignmentModalOpen,
  ] = useState(false);

  const [
    transferModalOpen,
    setTransferModalOpen,
  ] = useState(false);

  const loadData = async () => {
    if (!ticketId) {
      setError(
        "Talep numarası bulunamadı."
      );

      setLoading(false);

      return;
    }

    try {
      setLoading(true);
      setError("");

      const [
        ticketData,
        commentsData,
        eventsData,
        departmentsData,
      ] = await Promise.all([
        getTicketById(ticketId),
        getTicketComments(ticketId),
        getTicketEvents(ticketId),
        getDepartments(),
      ]);

      setTicket(ticketData);
      setComments(commentsData);
      setEvents(eventsData);
      setDepartments(departmentsData);
    } catch (err) {
      console.error(err);

      setError(
        getErrorMessage(
          err,
          "Talep bilgileri yüklenirken bir hata oluştu."
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [ticketId]);

  const handleAddComment = async (
    content: string,
    isInternal: boolean
  ) => {
    if (!ticketId) {
      return;
    }

    try {
      await addTicketComment(
        ticketId,
        content,
        isInternal
      );

      const [
        updatedComments,
        updatedEvents,
      ] = await Promise.all([
        getTicketComments(ticketId),
        getTicketEvents(ticketId),
      ]);

      setComments(updatedComments);
      setEvents(updatedEvents);
    } catch (err) {
      console.error(err);

      throw err;
    }
  };

  const handleStatusAndPrioritySave =
    async (
      status: string,
      priority: number
    ) => {
      if (!ticketId || !ticket) {
        return;
      }

      try {
        setSaving(true);
        setError("");

        if (
          status !== ticket.status
        ) {
          await changeTicketStatus(
            ticketId,
            status
          );
        }

        if (
          priority !== ticket.priority
        ) {
          await changeTicketPriority(
            ticketId,
            priority
          );
        }

        await loadData();
      } catch (err) {
        console.error(
          "Ticket update error:",
          err
        );

        setError(
          getErrorMessage(
            err,
            "Talep güncellenirken bir hata oluştu."
          )
        );
      } finally {
        setSaving(false);
      }
    };

  const handleAssign = async (
    assignedUserId: string
  ) => {
    if (!ticketId) {
      return;
    }

    try {
      setSaving(true);
      setError("");

      await assignTicket(
        ticketId,
        assignedUserId
      );

      await loadData();
    } catch (err) {
      console.error(
        "Ticket assignment error:",
        err
      );

      setError(
        getErrorMessage(
          err,
          "Talep atanırken bir hata oluştu."
        )
      );

      throw err;
    } finally {
      setSaving(false);
    }
  };

  const handleTransfer = async (
    departmentId: string
  ) => {
    if (!ticketId) {
      return;
    }

    try {
      setSaving(true);
      setError("");

      await transferTicket(
        ticketId,
        departmentId
      );

      await loadData();
    } catch (err) {
      console.error(
        "Ticket transfer error:",
        err
      );

      setError(
        getErrorMessage(
          err,
          "Talep transfer edilirken bir hata oluştu."
        )
      );

      throw err;
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="page">
        <div className="loading">
          Talep yükleniyor...
        </div>
      </div>
    );
  }

  if (error && !ticket) {
    return (
      <div className="page">
        <div className="card">
          <div className="error">
            {error}
          </div>

          <div className="form-actions">
            <button
              type="button"
              className="secondary-button"
              onClick={() =>
                navigate("/tickets")
              }
            >
              Taleplere Dön
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (!ticket) {
    return (
      <div className="page">
        <div className="card">
          <div className="empty">
            Talep bulunamadı.
          </div>
        </div>
      </div>
    );
  }

  const getPriorityText = (
    priority: number
  ) => {
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
        return String(priority);
    }
  };

  const getStatusText = (
    status: string
  ) => {
    switch (status) {
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
        return status;
    }
  };

  return (
    <div className="page">
      <div className="page-header">
        <div>
          <div className="eyebrow">
            Talep Detayı
          </div>

          <h1>
            {ticket.ticketNumber ||
              ticket.id}
          </h1>

          <p>
            Talebin detaylarını ve geçmiş
            işlemlerini görüntüleyin.
          </p>
        </div>

        <button
          type="button"
          className="secondary-button"
          onClick={() =>
            navigate("/tickets")
          }
        >
          Taleplere Dön
        </button>
      </div>

      {error && (
        <div className="card">
          <div className="error">
            {error}
          </div>
        </div>
      )}

      <div className="detail-grid">
        <div className="card">
          <div className="card-header">
            <div>
              <h2>
                {ticket.subject}
              </h2>

              <p>
                Talep bilgileri
              </p>
            </div>
          </div>

          <div className="form-grid">
            <div className="form-group">
              <label>Durum</label>

              <div className="readonly-value">
                {getStatusText(
                  ticket.status
                )}
              </div>
            </div>

            <div className="form-group">
              <label>Öncelik</label>

              <div className="readonly-value">
                {getPriorityText(
                  ticket.priority
                )}
              </div>
            </div>

            <div className="form-group">
              <label>Departman</label>

              <div className="readonly-value">
                {ticket.department?.name ||
                  "Atanmamış"}
              </div>
            </div>

            <div className="form-group">
              <label>Kategori</label>

              <div className="readonly-value">
                {ticket.category?.name ||
                  "Belirtilmemiş"}
              </div>
            </div>

            <div className="form-group">
              <label>Müşteri</label>

              <div className="readonly-value">
                {ticket.customer?.name ||
                  "Belirtilmemiş"}
              </div>
            </div>

            <div className="form-group">
              <label>
                Atanan kullanıcı
              </label>

              <div className="readonly-value">
                {ticket.assignedUserId ||
                  "Atanmamış"}
              </div>
            </div>
          </div>

          <div className="form-group">
            <label>Açıklama</label>

            <div className="description-box">
              {ticket.description}
            </div>
          </div>

          {ticket.createdAt && (
            <div className="muted">
              Oluşturulma:{" "}
              {new Date(
                ticket.createdAt
              ).toLocaleString(
                "tr-TR"
              )}
            </div>
          )}

          {ticket.updatedAt && (
            <div className="muted">
              Son güncelleme:{" "}
              {new Date(
                ticket.updatedAt
              ).toLocaleString(
                "tr-TR"
              )}
            </div>
          )}

          <div
            className="form-actions"
            style={{
              marginTop: 20,
              gap: 8,
            }}
          >
            <button
              type="button"
              className="primary-button"
              onClick={() =>
                setAssignmentModalOpen(
                  true
                )
              }
            >
              Talebi Ata
            </button>

            <button
              type="button"
              className="secondary-button"
              onClick={() =>
                setTransferModalOpen(
                  true
                )
              }
            >
              Departmana Transfer Et
            </button>
          </div>
        </div>

        <TicketStatusEditor
          currentStatus={
            ticket.status
          }
          currentPriority={
            ticket.priority
          }
          onSave={
            handleStatusAndPrioritySave
          }
        />
      </div>

      {saving && (
        <div className="muted">
          Talep güncelleniyor...
        </div>
      )}

      <TicketComments
        comments={comments}
        onAddComment={
          handleAddComment
        }
      />

      <TicketEventTimeline
        events={events}
      />

      <TicketAssignmentModal
        isOpen={
          assignmentModalOpen
        }
        ticketId={ticket.id}
        currentAssignedUserId={
          ticket.assignedUserId
        }
        onClose={() =>
          setAssignmentModalOpen(
            false
          )
        }
        onAssign={handleAssign}
      />

      <TicketTransferModal
        isOpen={
          transferModalOpen
        }
        ticketId={ticket.id}
        departments={
          departments
        }
        currentDepartmentId={
          ticket.departmentId
        }
        onClose={() =>
          setTransferModalOpen(
            false
          )
        }
        onTransfer={
          handleTransfer
        }
      />
    </div>
  );
}

function getErrorMessage(
  error: unknown,
  fallback: string
): string {
  if (
    error &&
    typeof error === "object" &&
    "response" in error
  ) {
    const response = (
      error as {
        response?: {
          data?: any;
          status?: number;
        };
      }
    ).response;

    const data = response?.data;

    if (typeof data === "string") {
      return data;
    }

    if (
      data?.message &&
      typeof data.message === "string"
    ) {
      return data.message;
    }

    if (
      data?.title &&
      typeof data.title === "string"
    ) {
      return data.title;
    }

    if (
      data?.detail &&
      typeof data.detail === "string"
    ) {
      return data.detail;
    }

    if (response?.status) {
      return `${fallback} (HTTP ${response.status})`;
    }
  }

  if (error instanceof Error) {
    return error.message;
  }

  return fallback;
}