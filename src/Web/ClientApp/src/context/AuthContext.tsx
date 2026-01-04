import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { Patient } from '../lib/types';
import { api } from '../lib/api';

interface AuthContextType {
    patient: Patient | null;
    login: (phone: string, fullName: string) => Promise<void>;
    logout: () => void;
    isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [patient, setPatient] = useState<Patient | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        // Check local storage on mount
        const storedPatient = localStorage.getItem('hms_patient');
        if (storedPatient) {
            try {
                setPatient(JSON.parse(storedPatient));
            } catch (e) {
                localStorage.removeItem('hms_patient');
            }
        }
        setIsLoading(false);
    }, []);

    const login = async (phone: string, fullName: string) => {
        try {
            // 1. Find patient by phone
            const patientData = await api.lookupPatient(phone);

            // 2. Simple verification match (case insensitive)
            if (patientData.fullName.toLowerCase() !== fullName.trim().toLowerCase()) {
                throw new Error("Name does not match our records for this phone number");
            }

            setPatient(patientData);
            localStorage.setItem('hms_patient', JSON.stringify(patientData));
        } catch (err) {
            throw err;
        }
    };

    const logout = () => {
        setPatient(null);
        localStorage.removeItem('hms_patient');
    };

    return (
        <AuthContext.Provider value={{ patient, login, logout, isLoading }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}
