function LoadingButton({
  children,
  className = 'btn btn-primary',
  disabled = false,
  isLoading = false,
  loadingText = 'Processando...',
  type = 'button',
  ...props
}) {
  return (
    <button className={className} disabled={disabled || isLoading} type={type} {...props}>
      {isLoading ? loadingText : children}
    </button>
  )
}

export default LoadingButton
