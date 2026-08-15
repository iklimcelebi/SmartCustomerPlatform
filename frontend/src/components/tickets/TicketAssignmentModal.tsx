import { useEffect, useState } from "react";
import Modal from "../common/Modal";

interface TicketAssignmentModalProps {
  isOpen: boolean;
  ticketId?: string;
  currentAssignedUserId?: string;
  onClose: () => void;
  onAssign?: (
    assignedUserId: string
  ) => Promise<void> | void;
}

export default function TicketAssignmentModal({
  isOpen,
  currentAssignedUserId,
  onClose,
  onAssign,
}: TicketAssignmentModalProps) {
  const [assignedUserId, setAssignedUserId] =
    useState("");

  const [loading, setLoading] =
    useState(false);

  useEffect(() => {
    if (isOpen) {
      setAssignedUserId(
        currentAssignedUserId || ""
      );
    }
  }, [
    isOpen,
    currentAssignedUserId,
  ]);

  const handleAssign = async () => {
    const trimmedUserId =
      assignedUserId.trim();

    if (!trimmedUserId || !onAssign) {
      return;
    }

    try {
      setLoading(true);

      await onAssign(trimmedUserId);

      setAssignedUserId("");
      onClose();
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      title="Talep Atama"
      onClose={onClose}
    >
      <div>
        <div className="form-group">
          <label htmlFor="assignment-user">
            Atanacak kullanıcı ID
          </label>

          <input
            id="assignment-user"
            type="text"
            value={assignedUserId}
            onChange={(e) =>
              setAssignedUserId(
                e.target.value
              )
            }
            placeholder="Kullanıcı GUID girin"
            disabled={loading}
          />

          <div
            className="muted"
            style={{
              marginTop: 8,
            }}
          >
            Sistemde henüz kullanıcı listeleme
            ekranı bulunmadığı için kullanıcı ID'si
            manuel girilmektedir.
          </div>
        </div>

        <div
          className="form-actions"
          style={{
            marginTop: 20,
            gap: 8,
          }}
        >
          <button
            type="button"
            className="secondary-button"
            onClick={onClose}
            disabled={loading}
          >
            Vazgeç
          </button>

          <button
            type="button"
            className="primary-button"
            disabled={
              !assignedUserId.trim() ||
              loading
            }
            onClick={handleAssign}
          >
            {loading
              ? "Atanıyor..."
              : "Ata"}
          </button>
        </div>
      </div>
    </Modal>
  );
}