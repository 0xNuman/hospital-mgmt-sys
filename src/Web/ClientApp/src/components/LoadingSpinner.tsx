import './LoadingSpinner.css';

interface LoadingSpinnerProps {
    size?: 'sm' | 'md' | 'lg';
    text?: string;
}

export default function LoadingSpinner({ size = 'md', text }: LoadingSpinnerProps) {
    return (
        <div className="loading-container">
            <div className={`spinner spinner-${size}`}></div>
            {text && <p className="loading-text">{text}</p>}
        </div>
    );
}
