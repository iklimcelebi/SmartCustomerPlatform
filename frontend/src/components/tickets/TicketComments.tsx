import { useState } from "react";

import type { TicketComment } from "../../types";

type Props = {
  comments: TicketComment[];
  onAddComment: (
    content: string,
    isInternal: boolean
  ) => Promise<void>;
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

export default function TicketComments({
  comments,
  onAddComment,
}: Props) {
  const [content, setContent] = useState("");
  const [isInternal, setIsInternal] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async () => {
    if (!content.trim()) {
      return;
    }

    try {
      setLoading(true);

      await onAddComment(
        content.trim(),
        isInternal
      );

      setContent("");
      setIsInternal(false);
    } finally {
      setLoading(false);
    }
  };

  const getAuthorName = (
    comment: TicketComment
  ) => {
    return (
      comment.author ||
      comment.userName ||
      "Kullanıcı"
    );
  };

  const isInternalComment = (
    comment: TicketComment
  ) => {
    if (typeof comment.isInternal === "boolean") {
      return comment.isInternal;
    }

    return comment.type === 2;
  };

  return (
    <div className="card">
      <div className="card-header">
        <div>
          <h2>Yorumlar</h2>

          <p>
            Talep üzerindeki müşteri ve ekip yorumları.
          </p>
        </div>
      </div>

      <div className="category-list">
        {comments.length === 0 ? (
          <div className="empty">
            Henüz yorum bulunmuyor.
          </div>
        ) : (
          comments.map((comment) => {
            const internal =
              isInternalComment(comment);

            return (
              <div
                className="category-item"
                key={comment.id}
              >
                <div className="category-main">
                  <div>
                    <strong>
                      {getAuthorName(comment)}
                    </strong>

                    {internal && (
                      <span className="code">
                        Dahili
                      </span>
                    )}

                    <p>{comment.content}</p>
                  </div>

                  {comment.createdAt && (
                    <span className="muted">
                      {formatTurkeyDate(
                        comment.createdAt
                      )}
                    </span>
                  )}
                </div>
              </div>
            );
          })
        )}

        <div className="form-grid single">
          <div className="form-group">
            <label>Yeni yorum</label>

            <textarea
              rows={4}
              value={content}
              onChange={(e) =>
                setContent(e.target.value)
              }
              placeholder="Yorumunuzu yazın..."
            />
          </div>

          <div className="form-group">
            <label>
              <input
                type="checkbox"
                checked={isInternal}
                onChange={(e) =>
                  setIsInternal(e.target.checked)
                }
              />{" "}
              Dahili not olarak ekle
            </label>
          </div>

          <div className="form-actions">
            <button
              type="button"
              className="primary-button"
              disabled={
                !content.trim() || loading
              }
              onClick={handleSubmit}
            >
              {loading
                ? "Ekleniyor..."
                : "Yorum Ekle"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}