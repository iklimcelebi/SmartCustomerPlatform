import { useEffect, useState } from "react";
import Modal from "../common/Modal";

interface Department {
  id: string;
  name: string;
}

export interface TicketTransferModalProps {
  isOpen: boolean;
  ticketId: string;
  departments?: Department[];
  currentDepartmentId?: string;
  onClose: () => void;
  onTransfer?: (
    departmentId: string
  ) => Promise<void> | void;
}

export default function TicketTransferModal({
  isOpen,
  departments = [],
  currentDepartmentId,
  onClose,
  onTransfer,
}: TicketTransferModalProps) {
  const [departmentId, setDepartmentId] =
    useState("");

  const [loading, setLoading] =
    useState(false);

  useEffect(() => {
    if (isOpen) {
      setDepartmentId(
        currentDepartmentId || ""
      );
    }
  }, [
    isOpen,
    currentDepartmentId,
  ]);

  const handleTransfer = async () => {
    if (!departmentId || !onTransfer) {
      return;
    }

    try {
      setLoading(true);

      await onTransfer(departmentId);

      setDepartmentId("");
      onClose();
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      title="Talep Transferi"
      onClose={onClose}
    >
      <div>
        <div className="form-group">
          <label htmlFor="transfer-department">
            Yeni departman
          </label>

          <select
            id="transfer-department"
            value={departmentId}
            onChange={(e) =>
              setDepartmentId(
                e.target.value
              )
            }
            disabled={loading}
          >
            <option value="">
              Departman seçin
            </option>

            {departments.map(
              (department) => (
                <option
                  key={department.id}
                  value={department.id}
                >
                  {department.name}
                </option>
              )
            )}
          </select>
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
              !departmentId ||
              loading ||
              departmentId ===
                currentDepartmentId
            }
            onClick={handleTransfer}
          >
            {loading
              ? "Transfer ediliyor..."
              : "Transfer Et"}
          </button>
        </div>
      </div>
    </Modal>
  );
}