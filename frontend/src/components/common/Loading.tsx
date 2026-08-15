type Props = {
  message?: string;
};

export default function Loading({
  message = "Yükleniyor...",
}: Props) {
  return (
    <div className="loading">
      {message}
    </div>
  );
}
