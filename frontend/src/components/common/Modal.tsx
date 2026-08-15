import type { ReactNode } from "react";

type Props = {
  isOpen: boolean;
  title: string;
  children: ReactNode;
  onClose: () => void;
  width?: string;
};

export default function Modal({
  isOpen,
  title,
  children,
  onClose,
  width = "520px",
}: Props) {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div
        className="modal"
        style={{ maxWidth: width }}
        onClick={(event) => event.stopPropagation()}
      >
        <div className="card-header">
          <div>
            <h2>{title}</h2>
          </div>

          <button
            type="button"
            className="secondary-button"
            onClick={onClose}
          >
            Kapat
          </button>
        </div>

        <div className="modal-content">
          {children}
        </div>
      </div>
    </div>
  );
}
