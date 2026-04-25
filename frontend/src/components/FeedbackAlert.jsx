function FeedbackAlert({ message, type = 'success' }) {
  if (!message) {
    return null
  }

  return (
    <div className={`alert alert-${type}`} role="alert">
      {message}
    </div>
  )
}

export default FeedbackAlert
